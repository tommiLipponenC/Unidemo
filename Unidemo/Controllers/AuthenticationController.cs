using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Unidemo.Data;
using Unidemo.DTOauthentication;
using Univ.Services;

namespace Unidemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenProps _tokenProps;
        
        public AuthenticationController(UserManager<ApplicationUser> userManager, TokenProps tokenProps)
        {
            _userManager = userManager;
            _tokenProps = tokenProps;
        }

        // /api/authentication/register
        ///<summary>Creates a new registered user.</summary>
        ///<remarks>
        ///Names may contain 2 - 50 letters, some characters are not allowed.
        ///Password min. 6 characters and contain uppercase, lowercase and special character like $. Example:  1A%c12.
        ///Email address must be correctly formatted. Example: bo@example.com.
        ///</remarks>
        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RegisterResponseDTO>> Register([FromBody] RegisterDTO model)
        {
            var userByEmail = await _userManager.FindByEmailAsync(model.Email);

            if (!ModelState.IsValid)
            {
                return BadRequest("Check properties");

            }

            if (userByEmail != null)
            {
                return new RegisterResponseDTO
                {
                    Success = false,
                    Message = "User allready exists"
                };
            }

       
            if (model.Password != model.ConfirmPassword)
                return new RegisterResponseDTO
                {
                    Message = "Confirm Password doesn't match.",
                    Success = false,
                };
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "RegisteredUser");

                return new RegisterResponseDTO
                {
                    Success = true,
                    Message = "New user is registered."
                };
            }

            return new RegisterResponseDTO
            {
                Message = "User registration failed.",
                Success = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }


        // /api/authentication/login
        ///<summary>Registered user login.</summary>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new LoginResponseDTO
                {
                    Message = "There is no user with that Email.",
                    Success = false,
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
                return new LoginResponseDTO
                {
                    Message = "Please check the Password and Username.",
                    Success = false,
                };

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)                           // Looppaa userin roolit ja lisää ne <Claims> tyyppiseen listaan
            {
                userClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwttoken = CreateToken(userClaims);             // Metodi alempana
            var refreshToken = GenerateRefreshToken();          // Metodi alempana
          
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);    //Refreshtokenin voimassaoloaika

            await _userManager.UpdateAsync(user);            // Vie Refreshtoken ja sen voimassaoloaika kantaan.

            return Ok(new
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwttoken),     // Luo Accesstokenin joka sisältää UserClaimssit
                RefreshToken = refreshToken,
                Expiration = jwttoken.ValidTo
            });            
        }

        // /api/authentication/refresh-token
        ///<summary>Replace expired access token and refresh token.</summary>
        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenDTO tokenDTO)
        {         
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request");
            }

            string? accessToken = tokenDTO.AccessToken;
            string? refreshToken = tokenDTO.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }
            string username = principal.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest("Invalid access token or refresh token");
            }
            var newAccessToken = CreateToken(principal.Claims.ToList());
            var newRefreshtoken = GenerateRefreshToken();

            user.RefreshToken = newRefreshtoken;
            await _userManager.UpdateAsync(user);

            return new ObjectResult(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshtoken
            });
        }

        // /api/authentication/revoke/tom@example.com
        ///<summary>Revokes refreshtoken of specified user name.</summary>
        [HttpPost]
        [Route("revoke/{username}")]                                      
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("Invalid user name");
            }
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);
            return NoContent();
        }

        // /api/authentication/revoke-all
        ///<summary>Revokes refresh tokens of all registered users.</summary>
        [HttpPost]
        [Route("revoke-all")]
        public async Task<IActionResult> RevokeAll()
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null; 
                await _userManager.UpdateAsync(user);
            }
            return NoContent();
        }

        private JwtSecurityToken CreateToken(List<Claim> userClaims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenProps.Key));
            var jwttoken = new JwtSecurityToken(
              audience: _tokenProps.Audience,
              issuer: _tokenProps.Issuer,
              claims: userClaims,
              expires: DateTime.UtcNow.AddMinutes(30),                      // Accesstokenin voimassaoloaika    
              signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return jwttoken;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,          // Oikeassa sovelluksessa Audience ja Issuer pitää tarkistaa.
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenProps.Key)),
                ValidateLifetime = false,         // Tokeni on vanhentunut, Lifetimen tarkistus olisi aina false.   
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }

    }
}

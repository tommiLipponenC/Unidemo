using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unidemo.Data;
using Unidemo.DTOcourse;
using Unidemo.DTOuser;


namespace Unidemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class UsersRolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UsersRolesController(ApplicationDbContext context,  UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }
   
        // GET: api/Users
        ///<summary>Retrieves a list of users in a specific Role.</summary>
        [HttpGet("role-id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUsersInRole(string id)
        {
            if (id == null)
            {
                return BadRequest("Check properties.");
            }
            try
            {
                var role = _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return BadRequest("Requested Role does not exist in records.");
                }
                var roleName = role.Result.Name;
                var roleUsers = await _userManager.GetUsersInRoleAsync(roleName);
                if (roleUsers == null)
                {
                    return BadRequest("There are no Users in this role yet.");
                }
                var results = _mapper.Map<IList<UserDto>>(roleUsers);
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error, please try again later.");
            }           
        }

        ///<summary>Retrieves a list of users Roles.</summary>
        [HttpGet("user-id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUsersRoles(string id)
        {
            if (id == null)
            {
                return BadRequest("Check properties.");
            }
            try
            {
               var user = await _context.Users.FindAsync(id);
                
                if (user == null)
                {
                    return BadRequest("Requested User does not exist in records.");
                }
                var roles = await _userManager.GetRolesAsync(user);

                if (roles == null)
                {
                    return BadRequest("Requested user has no Roles granted.");
                }
                return Ok(roles);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error, please try again later.");
            }
        }

        ///<summary>Grants specified Role to User by RoleId and UserId</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddUserToRole( string userId, string roleId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return BadRequest("User does not exist in records");
                }
                var role = _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == roleId);

                if (role == null)
                {
                    return BadRequest("Role does not exist in records");
                }
                var result = role.Result.Name.ToString();
               
                await _userManager.AddToRoleAsync(user, result);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error, please try again later.");
            }
        }
    }
}

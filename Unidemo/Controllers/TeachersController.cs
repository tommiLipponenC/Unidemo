using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Unidemo.Data;
using Unidemo.DTOteacher;

namespace Unidemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TeachersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
       
        private readonly IMapper _mapper;

        public TeachersController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
           
        }

        ///<summary>Retrieves details of current logged in user(required Role=Teacher).</summary>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTeacherDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) 
            {
                return NotFound();
            }
            var teacher = await _context.Users
                .Include(d => d.Department)
                .Where(u => u.Id == userId)
                .Select(u => new TeacherDetailsDto()
                {
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    Email = u.Email,
                    DepartmentName = u.Department.DepartmentName
                }).ToListAsync();
            return Ok(teacher);
        }
    }
}

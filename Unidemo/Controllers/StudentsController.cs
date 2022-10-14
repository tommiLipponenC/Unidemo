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
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IMapper _mapper;

        public StudentsController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;

        }

        ///<summary>Retrieves details of current logged in user(required Role=Student).</summary>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }
            var student = await _context.Users
                .Include(d => d.Department)
                .Where(u => u.Id == userId)
                .Select(u => new TeacherDetailsDto()
                {
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    Email = u.Email,
                    DepartmentName = u.Department.DepartmentName
                }).ToListAsync();
            return Ok(student);
        }

        /////<summary>Adds current logged in user to Course(required Role=Student).</summary>
        //[Authorize]
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> AddStudentToCourse(Guid courseId)
        //{
        //    if (courseId == Guid.Empty)
        //    {
        //        return BadRequest();
        //    }
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var course = _context.Courses.Where(c => c.CourseId == courseId);
        //    var user = await _context.Users.FindAsync(userId);
        //    user.CourseId = courseId;
        //    await _context.SaveChangesAsync();
        //    return Ok();          
        //}
    }
}


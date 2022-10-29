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
using Unidemo.DTOgrade;
using Unidemo.DTOteacher;
using Unidemo.Models;

namespace Unidemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(Roles = "Teacher, Admin, SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [Authorize(Roles = "Teacher")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTeacherDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) 
            {
                return NotFound();
            }
            try
            {
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
            catch (Exception)
            {

                return StatusCode(500, "Internal Server Error, please try again later.");
            }
       
        }

        ///<summary>Add Grade record to Student.(Required Role = Teacher)</summary>
        [Authorize(Roles = "Teacher")]
        [HttpPost]        
        public async Task<ActionResult<CreateGradeDto>> AddGrade(CreateGradeDto grade)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var teacher = await _userManager.FindByIdAsync(userId);
            if (teacher.Email != grade.TeacherName)
            {
                return BadRequest("Check TeacherName, only current logged in Teacher is accepted.");
            }
            var newGrade = _mapper.Map<Grade>(grade);

            await _context.AddAsync(newGrade);
            await _context.SaveChangesAsync();
            //  return CreatedAtAction("GetCourse", new { id = newCourse.CourseId }, newCourse);
            return this.Ok();
        }
    }
}

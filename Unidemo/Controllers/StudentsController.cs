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
using Unidemo.DTOcourse;
using Unidemo.DTOstudent;
using Unidemo.DTOstudentcourse;
using Unidemo.DTOteacher;
using Unidemo.Models;

namespace Unidemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(Roles = "Admin, SuperAdmin, Student")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
       
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }

            try
            {
                var student = await _userManager.Users
                .Include(c => c.Courses)         // Bridge table StudentCourses
                .Where(u => u.Id == userId)
                .Select(uc => new StudentDetailDto()
                {
                    Id = uc.Id,
                    Firstname = uc.Firstname,
                    Lastname = uc.Lastname,
                    Email = uc.Email,
                    StartDate = uc.StartDate,
                    GraduationDate = uc.GraduationDate,
                    Coursesss = uc.Courses.Select(x => new CourseDto
                    {
                        CourseId = x.CourseId,
                        CourseName = x.CourseName,
                        Points = x.Points,
                    }).ToArray()
                }).ToListAsync();

                return Ok(student);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error, please try again later.");
            }            
        }

        ///<summary>Adds current logged in user to Course(required Role=Student).</summary>
        [Authorize(Roles = "Student")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddStudentToCourse(StudentToCourseDto studentToCourseDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                StudentCourse studentCourse = new StudentCourse()
                {
                    CourseId = studentToCourseDto.CourseId,
                    Id = userId
                };
                await _context.AddRangeAsync(studentCourse);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error, please try again later.");
            }       
            return Ok();
        }

        // GET: api/Gradesss/5
        [Authorize(Roles = "Student")]
        [HttpGet("grades")]
        public async Task<IActionResult> GetGrade()
        {
            var email =   User.FindFirstValue(ClaimTypes.Email);                  
            var grade = await _context.Grades.Where(u => u.StudentName == email).ToListAsync();
    
            if (grade == null)
            {
                return NotFound("There are no Grade records for this user.");
            }

            return Ok(grade);
        }
    }
}


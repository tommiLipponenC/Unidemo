using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unidemo.Data;
using Unidemo.DTOcourse;
using Unidemo.Models;

namespace Unidemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(Roles = "Student, Admin, SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CoursesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Courses
        ///<summary>Retrieves a list of all Courses.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCourses()
        {
            try
            {
                var courses = await _context.Courses.ToListAsync();
                var results = _mapper.Map<IList<CourseDto>>(courses);
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error, please try again later.");
            }          
        }

        // GET: api/Courses/5
        ///<summary>Retrieves a specific course by id</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCourse(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }
            try
            {
                var result = _mapper.Map<CourseDto>(course);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error, please try again later.");
            }           
        }

        // PUT: api/Courses/5
        ///<summary>Edit course(Required Role = Admin).</summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditCourse(Guid id, [FromBody] EditCourseDto editCourseDto)
        {
            if (!ModelState.IsValid || id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                {
                    return NotFound("This Course doesn't exist in records.");
                }
                _mapper.Map(editCourseDto, course);
                _context.Entry(course).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!CourseExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {                   
                        foreach(var entry in ex.Entries)
                        {
                            if (entry.Entity is Course)
                            {
                                var proposedValues = entry.CurrentValues;
                                var databaseValues = entry.GetDatabaseValues();

                                foreach(var property in proposedValues.Properties)
                                {
                                    var proposedValue = proposedValues[property];
                                    var databaseValue = databaseValues[property];
                                }
                                entry.OriginalValues.SetValues(databaseValues);
                            }
                            else
                            {
                                throw new NotSupportedException(
                                    "Internal Server problem with saving changes, please try again later" + entry.Metadata.Name);
                            }
                        }                      
                    }
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error, please try again later.");
            }
            return NoContent();
        }

        // POST: api/Courses
        ///<summary>Creates a new Course and returns a newly created Course(Required Role = Admin).</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCourse([FromBody]CreateCourseDto createCourseDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Some properties are not valid.");
            }

            var courseExists = _context.Courses.Where(c => c.CourseName == createCourseDto.CourseName).Any();

            if (courseExists)
            {
                return BadRequest("Course allready exists");
            }
            try
            {
                var newCourse = _mapper.Map<Course>(createCourseDto);
                await _context.AddAsync(newCourse);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetCourse", new { id = newCourse.CourseId }, newCourse);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error, please try again later.");
            }            
        }

        // DELETE: api/Courses/5
        ///<summary>Deletes Course by id(Required Role = Admin)</summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Please check the parameter.");
            }
            var badCourse = await _context.Courses.FindAsync(id);
            if (badCourse == null)
            {
                return NotFound();
            }
            _context.Courses.Remove(badCourse);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseExists(Guid id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }
    }
}

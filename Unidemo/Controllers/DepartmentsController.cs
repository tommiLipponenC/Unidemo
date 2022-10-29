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
using Unidemo.DTOdepartment;
using Unidemo.Models;

namespace Unidemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class DepartmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DepartmentsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

      //  GET: api/Departments
       ///<summary>Retrieves a list of all departments.</summary>
       [HttpGet]
       
        [ProducesResponseType(StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var departments = await _context.Departments.ToListAsync();
                var results = _mapper.Map<IList<DepartmentDto>>(departments);
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error, please try again later.");
            }
        }

        // GET: api/Departments/5
        ///<summary>Retrieves a specific department by id</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Department>> GetDepartment(Guid id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }
            try
            {
                var result = _mapper.Map<DepartmentDto>(department);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error, please try again later.");
            }           
        }

        // PUT: api/Departments/5
        ///<summary>Edit Department</summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EditDepartment(Guid id, [FromBody] EditDepartmentDto editDepartmentDto)
        {
            if (!ModelState.IsValid || id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var department = await _context.Departments.FindAsync(id);
                if (department == null)
                {
                    return NotFound("This Department doesn't exist in records.");
                }
                _mapper.Map(editDepartmentDto, department);
                _context.Entry(department).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!DepartmentExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        foreach (var entry in ex.Entries)
                        {
                            if (entry.Entity is Department)
                            {
                                var proposedValues = entry.CurrentValues;                  // Valitse alkuperäiset vai uudet arvot. 
                                var databaseValues = entry.GetDatabaseValues();

                                foreach (var property in proposedValues.Properties)
                                {
                                    var proposedValue = proposedValues[property];
                                    var databaseValue = databaseValues[property];
                                }
                                entry.OriginalValues.SetValues(databaseValues);         // Aseta alkuperäiset arvot.
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

        // POST: api/Departments
        ///<summary>Creates a new Department and returns a newly created Department.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> PostDepartment([FromBody] CreateDepartmentDto createDepartmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Some properties are not valid.");
            }

            var departmentExists = _context.Departments.Where(d => d.DepartmentName == createDepartmentDto.DepartmentName).Any();

            if (departmentExists)
            {
                return BadRequest("Course allready exists");
            }
            try
            {
                var newDepartment = _mapper.Map<Department>(createDepartmentDto);
                await _context.AddAsync(newDepartment);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetDepartment", new { id = newDepartment.DepartmentId }, newDepartment);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error, please try again later.");
            }                     
        }

        // DELETE: api/Departments/5
        ///<summary>Deletes Department by id</summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Please check the parameter.");
            }
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentExists(Guid id)
        {
            return _context.Departments.Any(e => e.DepartmentId == id);
        }
    }
}

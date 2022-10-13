using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unidemo.Data;
using Unidemo.DTOdepartment;
using Unidemo.DTOuser;

namespace Unidemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        // GET: api/Users
        ///<summary>Retrieves a list of all Users.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                var results = _mapper.Map<IList<UserDto>>(users);
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error, please try again later.");
            }
        }

        ///<summary>Signs a department to user.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddUsersDepartment(string id, Guid departmentId)
        {
            if (departmentId == Guid.Empty) 
            {
                return BadRequest("Please check ID values.");
            }
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                var department = await _context.Departments.FindAsync(departmentId);
                if (department == null)
                {
                    return NotFound();
                }
                user.DepartmentId = departmentId;
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {

                return StatusCode(500, "Internal Server Error, please try again later.");
            }           
        }

        // DELETE: api/Courses/5
        ///<summary>Deletes User by id</summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (id == string.Empty)
            {
                return BadRequest("Please check the parameter.");
            }
            var badUser = await _context.Users.FindAsync(id);
            if (badUser == null)
            {
                return NotFound();
            }
            _context.Users.Remove(badUser);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
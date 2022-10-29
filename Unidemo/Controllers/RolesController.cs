using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;
using Unidemo.Data;
using Unidemo.DTO;

namespace Unidemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(Roles = "SuperAdmin, Admin, Developer")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class RolesController : ControllerBase
    {
       
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public RolesController(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
           
            _roleManager = roleManager;
            _mapper = mapper;
        }

        // GET: api/Roles
        ///<summary>Retrieves a list of all roles.</summary>
        [HttpGet]
       
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var results = _mapper.Map<IList<RoleListDto>>(roles);
            return Ok(results);          
        }

        // GET: api/Roles/name
        ///<summary>Retrieves a specific role by name</summary>
        [HttpGet("{name}")]
       
        public async Task<ActionResult> GetRole(string name)
        {
            var role = await _roleManager.FindByNameAsync(name);
            var result = _mapper.Map<RoleListDto>(role);

            if (role == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST: api/Roles
        ///<summary>Creates a new role.</summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Authorize(Roles = "Developer")]
        public async Task<ActionResult<ResponseDto>> CreateRole([FromBody] RoleDto roleDto)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleDto.Name);
            if (roleExists)
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = ($"Role with name {roleDto.Name} allready exists")
                };
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Some properties are not valid.");
            }
            IdentityRole role = new()
            {
                Name = roleDto.Name,
            };
            IdentityResult result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return new ResponseDto
                {
                    Success = true,
                    Message = "New role was created."
                };
            }
            return new ResponseDto
            {
                Message = "Role creation failed.",
                Success = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        // PUT: api/Roles/name
        ///<summary>Edit role</summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        [Authorize(Roles = "Developer")]
        public async Task<IActionResult> EditRole(string id, [Bind("Id, Name")] EditRoleDto identityRole)
        {
            if (id != identityRole.Id)
            {
                return BadRequest("Please check all propreties.");
            }
               
            try
            {
                var role = await _roleManager.FindByIdAsync(identityRole.Id);
                role.Name = identityRole.Name;
                var result = await _roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    return BadRequest("Role update failed.");
                }
                return Accepted("Role was updated succesfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }                
            }
        }

        // DELETE: api/Roles/name
        ///<summary>Deletes role by name</summary>
        [HttpDelete("{name}")]
        [Authorize(Roles = "SuperAdmin")]
        [Authorize(Roles = "Developer")]
        public async Task<ActionResult<ResponseDto>> DeleteRole(string name)
        {
            var role = await _roleManager.FindByNameAsync(name);
            if (role is null)
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = ($"Role with name {name} was not found")                   
                };
            }
            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return new ResponseDto
                {
                    Success = true,
                    Message = ($"Role with name {name} was succesfully deleted")
                };
            }
            return NoContent();        
        }

        private bool RoleExists(string id)
        {
            return _roleManager.Roles.Any(e => e.Id == id);
        }
    }
}

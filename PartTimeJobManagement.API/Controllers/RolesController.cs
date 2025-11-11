using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Services;

namespace PartTimeJobManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Get all roles with OData support
        /// </summary>
        /// <remarks>
        /// Examples:
        /// - Filter: ?$filter=TotalUsers gt 0
        /// - Sort: ?$orderby=RoleName
        /// </remarks>
        [HttpGet]
        [AllowAnonymous]
        [EnableQuery(MaxTop = 50)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoleResponseDTO>>> GetAll()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Get role by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleResponseDTO>> GetById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound(new { message = "Role not found" });

            return Ok(role);
        }

        /// <summary>
        /// Get role by name
        /// </summary>
        [HttpGet("name/{name}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleResponseDTO>> GetByName(string name)
        {
            var role = await _roleService.GetRoleByNameAsync(name);
            if (role == null)
                return NotFound(new { message = "Role not found" });

            return Ok(role);
        }

        /// <summary>
        /// Create a new role (Admin only)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleResponseDTO>> Create([FromBody] CreateRoleDTO dto)
        {
            try
            {
                var role = await _roleService.CreateRoleAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = role.RoleId }, role);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update role (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleResponseDTO>> Update(int id, [FromBody] UpdateRoleDTO dto)
        {
            try
            {
                var role = await _roleService.UpdateRoleAsync(id, dto);
                return Ok(role);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a role (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (!result)
                return NotFound(new { message = "Role not found" });

            return NoContent();
        }
    }
}

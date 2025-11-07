using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Services;

namespace PartTimeJobManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployersController : ControllerBase
    {
        private readonly IEmployerService _employerService;

        public EmployersController(IEmployerService employerService)
        {
            _employerService = employerService;
        }

        /// <summary>
        /// Get all employers
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployerResponseDTO>>> GetAll()
        {
            var employers = await _employerService.GetAllEmployersAsync();
            return Ok(employers);
        }

        /// <summary>
        /// Get verified employers only
        /// </summary>
        [HttpGet("verified")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployerResponseDTO>>> GetVerified()
        {
            var employers = await _employerService.GetVerifiedEmployersAsync();
            return Ok(employers);
        }

        /// <summary>
        /// Get employer by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployerResponseDTO>> GetById(int id)
        {
            var employer = await _employerService.GetEmployerByIdAsync(id);
            if (employer == null)
                return NotFound(new { message = "Employer not found" });

            return Ok(employer);
        }

        /// <summary>
        /// Get employer by user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployerResponseDTO>> GetByUserId(int userId)
        {
            var employer = await _employerService.GetEmployerByUserIdAsync(userId);
            if (employer == null)
                return NotFound(new { message = "Employer not found" });

            return Ok(employer);
        }

        /// <summary>
        /// Create a new employer
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployerResponseDTO>> Create([FromBody] CreateEmployerDTO dto)
        {
            try
            {
                var employer = await _employerService.CreateEmployerAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = employer.EmployerId }, employer);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update employer information
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Employer,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployerResponseDTO>> Update(int id, [FromBody] UpdateEmployerDTO dto)
        {
            try
            {
                var employer = await _employerService.UpdateEmployerAsync(id, dto);
                return Ok(employer);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete an employer
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _employerService.DeleteEmployerAsync(id);
            if (!result)
                return NotFound(new { message = "Employer not found" });

            return NoContent();
        }
    }
}

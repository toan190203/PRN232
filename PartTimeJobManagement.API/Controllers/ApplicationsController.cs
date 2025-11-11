using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Services;

namespace PartTimeJobManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Get all applications with OData support (Admin only)
        /// </summary>
        /// <remarks>
        /// Examples:
        /// - Filter by status: ?$filter=Status eq 'Pending'
        /// - Sort: ?$orderby=AppliedAt desc
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [EnableQuery(MaxTop = 100)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApplicationResponseDTO>>> GetAll()
        {
            var applications = await _applicationService.GetAllApplicationsAsync();
            return Ok(applications);
        }

        /// <summary>
        /// Get application by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApplicationResponseDTO>> GetById(int id)
        {
            var application = await _applicationService.GetApplicationByIdAsync(id);
            if (application == null)
                return NotFound(new { message = "Application not found" });

            return Ok(application);
        }

        /// <summary>
        /// Get applications by student with OData support
        /// </summary>
        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Student,Admin")]
        [EnableQuery(MaxTop = 100)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApplicationResponseDTO>>> GetByStudent(int studentId)
        {
            var applications = await _applicationService.GetApplicationsByStudentAsync(studentId);
            return Ok(applications);
        }

        /// <summary>
        /// Get applications by job with OData support
        /// </summary>
        [HttpGet("job/{jobId}")]
        [Authorize(Roles = "Employer,Admin")]
        [EnableQuery(MaxTop = 100)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApplicationResponseDTO>>> GetByJob(int jobId)
        {
            var applications = await _applicationService.GetApplicationsByJobAsync(jobId);
            return Ok(applications);
        }

        /// <summary>
        /// Create a new job application (Student only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApplicationResponseDTO>> Create([FromBody] CreateApplicationDTO dto)
        {
            try
            {
                var application = await _applicationService.CreateApplicationAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = application.ApplicationId }, application);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update application status (Employer/Admin only)
        /// </summary>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Employer,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApplicationResponseDTO>> UpdateStatus(int id, [FromBody] UpdateApplicationStatusDTO dto)
        {
            try
            {
                var application = await _applicationService.UpdateApplicationStatusAsync(id, dto.Status);
                return Ok(application);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete an application
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Student,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _applicationService.DeleteApplicationAsync(id);
            if (!result)
                return NotFound(new { message = "Application not found" });

            return NoContent();
        }
    }
}

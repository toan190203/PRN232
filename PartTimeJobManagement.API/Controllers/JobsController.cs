using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Services;

namespace PartTimeJobManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        /// <summary>
        /// Get all jobs with OData support (filtering, sorting, paging)
        /// </summary>
        /// <remarks>
        /// Examples:
        /// - Filter: ?$filter=Salary gt 1000
        /// - Sort: ?$orderby=CreatedAt desc
        /// - Page: ?$top=10&amp;$skip=20
        /// - Select: ?$select=Title,Salary
        /// </remarks>
        [HttpGet]
        [EnableQuery(MaxTop = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<JobResponseDTO>>> GetAll()
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return Ok(jobs);
        }

        /// <summary>
        /// Get active jobs only with OData support
        /// </summary>
        [HttpGet("active")]
        [EnableQuery(MaxTop = 100)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<JobResponseDTO>>> GetActive()
        {
            var jobs = await _jobService.GetActiveJobsAsync();
            return Ok(jobs);
        }

        /// <summary>
        /// Get job by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JobResponseDTO>> GetById(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
                return NotFound(new { message = "Job not found" });

            return Ok(job);
        }

        /// <summary>
        /// Get jobs by employer with OData support
        /// </summary>
        [HttpGet("employer/{employerId}")]
        [EnableQuery(MaxTop = 100)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<JobResponseDTO>>> GetByEmployer(int employerId)
        {
            var jobs = await _jobService.GetJobsByEmployerAsync(employerId);
            return Ok(jobs);
        }

        /// <summary>
        /// Get jobs by category with OData support
        /// </summary>
        [HttpGet("category/{categoryId}")]
        [EnableQuery(MaxTop = 100)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<JobResponseDTO>>> GetByCategory(int categoryId)
        {
            var jobs = await _jobService.GetJobsByCategoryAsync(categoryId);
            return Ok(jobs);
        }

        /// <summary>
        /// Create a new job (Employer only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Employer,Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<JobResponseDTO>> Create([FromBody] CreateJobDTO dto)
        {
            try
            {
                var job = await _jobService.CreateJobAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = job.JobId }, job);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update job information (Employer only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Employer,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JobResponseDTO>> Update(int id, [FromBody] UpdateJobDTO dto)
        {
            try
            {
                var job = await _jobService.UpdateJobAsync(id, dto);
                return Ok(job);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a job (Employer/Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employer,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _jobService.DeleteJobAsync(id);
                if (!result)
                    return NotFound(new { message = "Job not found" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

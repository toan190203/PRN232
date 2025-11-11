using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Services;

namespace PartTimeJobManagement.API.Controllers
{
    // UNUSED API CONTROLLER - All endpoints commented out temporarily
    /*
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationHistoriesController : ControllerBase
    {
        private readonly IApplicationHistoryService _historyService;

        public ApplicationHistoriesController(IApplicationHistoryService historyService)
        {
            _historyService = historyService;
        }

        /// <summary>
        /// Get history by application ID
        /// </summary>
        [HttpGet("application/{applicationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApplicationHistoryResponseDTO>>> GetByApplicationId(int applicationId)
        {
            var histories = await _historyService.GetHistoryByApplicationIdAsync(applicationId);
            return Ok(histories);
        }

        /// <summary>
        /// Get history by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApplicationHistoryResponseDTO>> GetById(int id)
        {
            var history = await _historyService.GetHistoryByIdAsync(id);
            if (history == null)
                return NotFound(new { message = "History record not found" });

            return Ok(history);
        }

        /// <summary>
        /// Create application history record (automatically triggered on status change)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Employer,Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApplicationHistoryResponseDTO>> Create([FromBody] CreateApplicationHistoryDTO dto)
        {
            try
            {
                var history = await _historyService.CreateHistoryAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = history.HistoryId }, history);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
    */
}

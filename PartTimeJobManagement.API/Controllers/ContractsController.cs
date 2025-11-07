using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Services;

namespace PartTimeJobManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractsController(IContractService contractService)
        {
            _contractService = contractService;
        }

        /// <summary>
        /// Get all contracts (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContractResponseDTO>>> GetAll()
        {
            var contracts = await _contractService.GetAllContractsAsync();
            return Ok(contracts);
        }

        /// <summary>
        /// Get active contracts
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContractResponseDTO>>> GetActive()
        {
            var contracts = await _contractService.GetActiveContractsAsync();
            return Ok(contracts);
        }

        /// <summary>
        /// Get contract by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContractResponseDTO>> GetById(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null)
                return NotFound(new { message = "Contract not found" });

            return Ok(contract);
        }

        /// <summary>
        /// Get contract by application ID
        /// </summary>
        [HttpGet("application/{applicationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContractResponseDTO>> GetByApplicationId(int applicationId)
        {
            var contract = await _contractService.GetContractByApplicationIdAsync(applicationId);
            if (contract == null)
                return NotFound(new { message = "Contract not found" });

            return Ok(contract);
        }

        /// <summary>
        /// Get contracts by student ID
        /// </summary>
        [HttpGet("student/{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContractResponseDTO>>> GetByStudentId(int studentId)
        {
            var contracts = await _contractService.GetContractsByStudentIdAsync(studentId);
            return Ok(contracts);
        }

        /// <summary>
        /// Get contracts by employer ID
        /// </summary>
        [HttpGet("employer/{employerId}")]
        [Authorize(Roles = "Employer,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContractResponseDTO>>> GetByEmployerId(int employerId)
        {
            var contracts = await _contractService.GetContractsByEmployerIdAsync(employerId);
            return Ok(contracts);
        }

        /// <summary>
        /// Create a new contract (Employer/Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Employer,Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContractResponseDTO>> Create([FromBody] CreateContractDTO dto)
        {
            try
            {
                var contract = await _contractService.CreateContractAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = contract.ContractId }, contract);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update contract (Employer/Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Employer,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContractResponseDTO>> Update(int id, [FromBody] UpdateContractDTO dto)
        {
            try
            {
                var contract = await _contractService.UpdateContractAsync(id, dto);
                return Ok(contract);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a contract (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _contractService.DeleteContractAsync(id);
            if (!result)
                return NotFound(new { message = "Contract not found" });

            return NoContent();
        }
    }
}

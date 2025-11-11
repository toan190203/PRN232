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
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Get all payments with OData support (Admin only)
        /// </summary>
        /// <remarks>
        /// Examples:
        /// - Filter by amount: ?$filter=Amount gt 1000
        /// - Filter by status: ?$filter=PaymentStatus eq 'Paid'
        /// - Sort: ?$orderby=PaymentDate desc
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [EnableQuery(MaxTop = 100)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PaymentResponseDTO>>> GetAll()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(payments);
        }

        /// <summary>
        /// Get payment by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentResponseDTO>> GetById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
                return NotFound(new { message = "Payment not found" });

            return Ok(payment);
        }

        /// <summary>
        /// Get payments by contract with OData support
        /// </summary>
        [HttpGet("contract/{contractId}")]
        [EnableQuery(MaxTop = 100)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PaymentResponseDTO>>> GetByContract(int contractId)
        {
            var payments = await _paymentService.GetPaymentsByContractAsync(contractId);
            return Ok(payments);
        }

        /// <summary>
        /// Get payments by student ID with OData support
        /// </summary>
        [HttpGet("student/{studentId}")]
        [EnableQuery(MaxTop = 100)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PaymentResponseDTO>>> GetByStudent(int studentId)
        {
            var payments = await _paymentService.GetPaymentsByStudentAsync(studentId);
            return Ok(payments);
        }

        // UNUSED API - Commented out temporarily
        /*
        /// <summary>
        /// Get payments by status
        /// </summary>
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Employer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PaymentResponseDTO>>> GetByStatus(string status)
        {
            var payments = await _paymentService.GetPaymentsByStatusAsync(status);
            return Ok(payments);
        }
        */

        /// <summary>
        /// Create a new payment (Employer/Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Employer,Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaymentResponseDTO>> Create([FromBody] CreatePaymentDTO dto)
        {
            try
            {
                var payment = await _paymentService.CreatePaymentAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = payment.PaymentId }, payment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update payment (Student can confirm, Employer/Admin can edit)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Student,Employer,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentResponseDTO>> Update(int id, [FromBody] UpdatePaymentDTO dto)
        {
            try
            {
                var payment = await _paymentService.UpdatePaymentAsync(id, dto);
                return Ok(payment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a payment (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _paymentService.DeletePaymentAsync(id);
            if (!result)
                return NotFound(new { message = "Payment not found" });

            return NoContent();
        }
    }
}

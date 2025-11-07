using System.ComponentModel.DataAnnotations;

namespace PartTimeJobManagement.API.DTOs
{
    // Payment DTOs
    public class CreatePaymentDTO
    {
        [Required(ErrorMessage = "Contract ID is required")]
        public int ContractId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }
    }

    public class UpdatePaymentDTO
    {
        [Range(0.01, double.MaxValue)]
        public decimal? Amount { get; set; }

        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [RegularExpression("^(Pending|Completed|Failed|Refunded)$", 
            ErrorMessage = "Status must be Pending, Completed, Failed, or Refunded")]
        public string? Status { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }
    }

    public class PaymentResponseDTO
    {
        public int PaymentId { get; set; }
        public int ContractId { get; set; }
        public string StudentName { get; set; } = null!;
        public string JobTitle { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string Status { get; set; } = null!;
        public string? Description { get; set; }
    }
}

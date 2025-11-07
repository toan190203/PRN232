namespace PartTimeJobManagement.Client.Models.DTOs
{
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

    public class CreatePaymentDTO
    {
        public int ContractId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Description { get; set; }
    }

    public class UpdatePaymentDTO
    {
        public decimal? Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
    }
}

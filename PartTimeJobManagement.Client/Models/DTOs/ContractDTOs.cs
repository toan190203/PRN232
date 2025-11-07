namespace PartTimeJobManagement.Client.Models.DTOs
{
    public class ContractResponseDTO
    {
        public int ContractId { get; set; }
        public int ApplicationId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public int JobId { get; set; }
        public string JobTitle { get; set; } = null!;
        public string EmployerName { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public decimal SalaryAgreed { get; set; }
        public string? ContractFile { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int TotalPayments { get; set; }
        public decimal TotalPaid { get; set; }
    }

    public class CreateContractDTO
    {
        public int ApplicationId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public decimal SalaryAgreed { get; set; }
        public string? ContractFile { get; set; }
    }

    public class UpdateContractDTO
    {
        public DateOnly? EndDate { get; set; }
        public decimal? SalaryAgreed { get; set; }
        public string? ContractFile { get; set; }
        public string? Status { get; set; }
    }
}

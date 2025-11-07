using System.ComponentModel.DataAnnotations;

namespace PartTimeJobManagement.API.DTOs
{
    // Contract DTOs
    public class CreateContractDTO
    {
        [Required(ErrorMessage = "Application ID is required")]
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateOnly StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        [Required(ErrorMessage = "Salary is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
        public decimal SalaryAgreed { get; set; }

        [StringLength(256)]
        public string? ContractFile { get; set; }
    }

    public class UpdateContractDTO
    {
        public DateOnly? EndDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SalaryAgreed { get; set; }

        [StringLength(256)]
        public string? ContractFile { get; set; }

        [RegularExpression("^(Active|Completed|Terminated)$", 
            ErrorMessage = "Status must be Active, Completed, or Terminated")]
        public string? Status { get; set; }
    }

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
}

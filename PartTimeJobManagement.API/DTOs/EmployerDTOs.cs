using System.ComponentModel.DataAnnotations;

namespace PartTimeJobManagement.API.DTOs
{
    // Employer DTOs
    public class CreateEmployerDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Company name is required")]
        [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
        public string CompanyName { get; set; } = null!;

        [StringLength(100)]
        public string? ContactName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? TaxCode { get; set; }
    }

    public class UpdateEmployerDTO
    {
        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = null!;

        [StringLength(100)]
        public string? ContactName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? TaxCode { get; set; }

        public bool IsVerified { get; set; }
    }

    public class EmployerResponseDTO
    {
        public int EmployerId { get; set; }
        public string Email { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? TaxCode { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalJobs { get; set; }
    }
}

namespace PartTimeJobManagement.Client.Models.DTOs
{
    public class StudentResponseDTO
    {
        public int StudentId { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Major { get; set; }
        public int? YearOfStudy { get; set; }
        public string? Cvfile { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalApplications { get; set; }
    }

    public class CreateStudentProfileDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Major { get; set; }
        public int? YearOfStudy { get; set; }
        public string? Cvfile { get; set; }
    }

    public class UpdateStudentDTO
    {
        public string FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Major { get; set; }
        public int? YearOfStudy { get; set; }
        public string? Cvfile { get; set; }
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

    public class UpdateEmployerDTO
    {
        public string CompanyName { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? TaxCode { get; set; }
        public bool IsVerified { get; set; }
    }
}

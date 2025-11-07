using System.ComponentModel.DataAnnotations;

namespace PartTimeJobManagement.API.DTOs
{
    // Student DTOs
    public class CreateStudentDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = null!;

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? Major { get; set; }

        [Range(1, 6, ErrorMessage = "Year of study must be between 1 and 6")]
        public int? YearOfStudy { get; set; }

        public string? Cvfile { get; set; }
    }

    public class CreateStudentProfileDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? Major { get; set; }

        [Range(1, 6)]
        public int? YearOfStudy { get; set; }

        public string? Cvfile { get; set; }
    }

    public class UpdateStudentDTO
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? Major { get; set; }

        [Range(1, 6)]
        public int? YearOfStudy { get; set; }

        public string? Cvfile { get; set; }
    }

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
}

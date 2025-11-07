using System.ComponentModel.DataAnnotations;

namespace PartTimeJobManagement.API.DTOs
{
    // Application DTOs
    public class CreateApplicationDTO
    {
        [Required(ErrorMessage = "Student ID is required")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Job ID is required")]
        public int JobId { get; set; }

        [StringLength(2000, ErrorMessage = "Cover letter cannot exceed 2000 characters")]
        public string? CoverLetter { get; set; }
    }

    public class UpdateApplicationStatusDTO
    {
        [Required]
        [RegularExpression("^(Pending|Accepted|Rejected|Withdrawn)$", 
            ErrorMessage = "Status must be Pending, Accepted, Rejected, or Withdrawn")]
        public string Status { get; set; } = null!;

        public string? Note { get; set; }
    }

    public class ApplicationResponseDTO
    {
        public int ApplicationId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public int JobId { get; set; }
        public string JobTitle { get; set; } = null!;
        public string EmployerName { get; set; } = null!;
        public DateTime ApplicationDate { get; set; }
        public string? CoverLetter { get; set; }
        public string Status { get; set; } = null!;
    }
}

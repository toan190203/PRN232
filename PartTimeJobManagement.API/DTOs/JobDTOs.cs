using System.ComponentModel.DataAnnotations;

namespace PartTimeJobManagement.API.DTOs
{
    // Job DTOs
    public class CreateJobDTO
    {
        [Required(ErrorMessage = "Employer ID is required")]
        public int EmployerId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
        public decimal? Salary { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        public int? CategoryId { get; set; }

        public DateTime? ExpirationDate { get; set; }
    }

    public class UpdateJobDTO
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal? Salary { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        public int? CategoryId { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [RegularExpression("^(Active|Closed|Pending)$", ErrorMessage = "Status must be Active, Closed, or Pending")]
        public string Status { get; set; } = "Active";
    }

    public class JobResponseDTO
    {
        public int JobId { get; set; }
        public int EmployerId { get; set; }
        public string EmployerName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal? Salary { get; set; }
        public string? Location { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Status { get; set; } = null!;
        public int TotalApplications { get; set; }
    }
}

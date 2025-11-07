using System.ComponentModel.DataAnnotations;

namespace PartTimeJobManagement.API.DTOs
{
    // JobCategory DTOs
    public class CreateJobCategoryDTO
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = null!;
    }

    public class UpdateJobCategoryDTO
    {
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = null!;
    }

    public class JobCategoryResponseDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int TotalJobs { get; set; }
    }
}

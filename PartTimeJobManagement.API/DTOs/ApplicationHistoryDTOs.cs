using System.ComponentModel.DataAnnotations;

namespace PartTimeJobManagement.API.DTOs
{
    // ApplicationHistory DTOs
    public class CreateApplicationHistoryDTO
    {
        [Required(ErrorMessage = "Application ID is required")]
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50)]
        public string Status { get; set; } = null!;

        [StringLength(500)]
        public string? Note { get; set; }
    }

    public class ApplicationHistoryResponseDTO
    {
        public int HistoryId { get; set; }
        public int ApplicationId { get; set; }
        public string StudentName { get; set; } = null!;
        public string JobTitle { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime ChangedAt { get; set; }
        public string? Note { get; set; }
    }
}

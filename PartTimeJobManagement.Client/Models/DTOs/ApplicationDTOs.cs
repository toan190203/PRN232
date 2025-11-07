namespace PartTimeJobManagement.Client.Models.DTOs
{
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

    public class CreateApplicationDTO
    {
        public int StudentId { get; set; }
        public int JobId { get; set; }
        public string? CoverLetter { get; set; }
    }

    public class UpdateApplicationStatusDTO
    {
        public string Status { get; set; } = null!;
        public string? Note { get; set; }
    }
}

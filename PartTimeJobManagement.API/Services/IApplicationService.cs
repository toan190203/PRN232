using PartTimeJobManagement.API.DTOs;

namespace PartTimeJobManagement.API.Services
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationResponseDTO>> GetAllApplicationsAsync();
        Task<ApplicationResponseDTO?> GetApplicationByIdAsync(int id);
        Task<ApplicationResponseDTO> CreateApplicationAsync(CreateApplicationDTO dto);
        Task<ApplicationResponseDTO> UpdateApplicationStatusAsync(int id, string status);
        Task<bool> DeleteApplicationAsync(int id);
        Task<IEnumerable<ApplicationResponseDTO>> GetApplicationsByStudentAsync(int studentId);
        Task<IEnumerable<ApplicationResponseDTO>> GetApplicationsByJobAsync(int jobId);
    }
}

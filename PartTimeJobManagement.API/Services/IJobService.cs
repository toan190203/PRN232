using PartTimeJobManagement.API.DTOs;

namespace PartTimeJobManagement.API.Services
{
    public interface IJobService
    {
        Task<IEnumerable<JobResponseDTO>> GetAllJobsAsync();
        Task<JobResponseDTO?> GetJobByIdAsync(int id);
        Task<JobResponseDTO> CreateJobAsync(CreateJobDTO dto);
        Task<JobResponseDTO> UpdateJobAsync(int id, UpdateJobDTO dto);
        Task<bool> DeleteJobAsync(int id);
        Task<IEnumerable<JobResponseDTO>> GetActiveJobsAsync();
        Task<IEnumerable<JobResponseDTO>> GetJobsByEmployerAsync(int employerId);
        Task<IEnumerable<JobResponseDTO>> GetJobsByCategoryAsync(int categoryId);
    }
}

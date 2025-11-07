using PartTimeJobManagement.API.DTOs;

namespace PartTimeJobManagement.API.Services
{
    public interface IApplicationHistoryService
    {
        Task<IEnumerable<ApplicationHistoryResponseDTO>> GetHistoryByApplicationIdAsync(int applicationId);
        Task<ApplicationHistoryResponseDTO?> GetHistoryByIdAsync(int id);
        Task<ApplicationHistoryResponseDTO> CreateHistoryAsync(CreateApplicationHistoryDTO dto);
    }
}

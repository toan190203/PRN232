using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Services
{
    public interface IEmployerService
    {
        Task<IEnumerable<EmployerResponseDTO>> GetAllEmployersAsync();
        Task<EmployerResponseDTO?> GetEmployerByIdAsync(int id);
        Task<EmployerResponseDTO?> GetEmployerByUserIdAsync(int userId);
        Task<EmployerResponseDTO> CreateEmployerAsync(CreateEmployerDTO dto);
        Task<EmployerResponseDTO> UpdateEmployerAsync(int id, UpdateEmployerDTO dto);
        Task<bool> DeleteEmployerAsync(int id);
        Task<IEnumerable<EmployerResponseDTO>> GetVerifiedEmployersAsync();
    }
}

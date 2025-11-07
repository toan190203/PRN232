using PartTimeJobManagement.API.DTOs;

namespace PartTimeJobManagement.API.Services
{
    public interface IContractService
    {
        Task<IEnumerable<ContractResponseDTO>> GetAllContractsAsync();
        Task<ContractResponseDTO?> GetContractByIdAsync(int id);
        Task<ContractResponseDTO?> GetContractByApplicationIdAsync(int applicationId);
        Task<IEnumerable<ContractResponseDTO>> GetContractsByStudentIdAsync(int studentId);
        Task<IEnumerable<ContractResponseDTO>> GetContractsByEmployerIdAsync(int employerId);
        Task<ContractResponseDTO> CreateContractAsync(CreateContractDTO dto);
        Task<ContractResponseDTO> UpdateContractAsync(int id, UpdateContractDTO dto);
        Task<bool> DeleteContractAsync(int id);
        Task<IEnumerable<ContractResponseDTO>> GetActiveContractsAsync();
    }
}

using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public interface IContractRepository : IRepository<Contract>
    {
        Task<Contract?> GetContractWithDetailsAsync(int contractId);
        Task<Contract?> GetContractByApplicationIdAsync(int applicationId);
        Task<IEnumerable<Contract>> GetActiveContractsAsync();
        Task<IEnumerable<Contract>> GetContractsByStatusAsync(string status);
        Task<IEnumerable<Contract>> GetContractsByStudentIdAsync(int studentId);
        Task<IEnumerable<Contract>> GetContractsByEmployerIdAsync(int employerId);
    }
}

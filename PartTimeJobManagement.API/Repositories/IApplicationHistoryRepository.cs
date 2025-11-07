using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public interface IApplicationHistoryRepository : IRepository<ApplicationHistory>
    {
        Task<IEnumerable<ApplicationHistory>> GetHistoryByApplicationIdAsync(int applicationId);
        Task<ApplicationHistory?> GetLatestHistoryByApplicationIdAsync(int applicationId);
    }
}

using Microsoft.EntityFrameworkCore;
using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public class ApplicationHistoryRepository : Repository<ApplicationHistory>, IApplicationHistoryRepository
    {
        public ApplicationHistoryRepository(PartTimeJobManagementContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ApplicationHistory>> GetHistoryByApplicationIdAsync(int applicationId)
        {
            return await _dbSet
                .Where(h => h.ApplicationId == applicationId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }

        public async Task<ApplicationHistory?> GetLatestHistoryByApplicationIdAsync(int applicationId)
        {
            return await _dbSet
                .Where(h => h.ApplicationId == applicationId)
                .OrderByDescending(h => h.ChangedAt)
                .FirstOrDefaultAsync();
        }
    }
}

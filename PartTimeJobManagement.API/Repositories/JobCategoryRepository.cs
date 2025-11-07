using Microsoft.EntityFrameworkCore;
using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public class JobCategoryRepository : Repository<JobCategory>, IJobCategoryRepository
    {
        public JobCategoryRepository(PartTimeJobManagementContext context) : base(context)
        {
        }

        public async Task<JobCategory?> GetCategoryByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.CategoryName == name);
        }

        public async Task<IEnumerable<JobCategory>> GetCategoriesWithJobsAsync()
        {
            return await _dbSet
                .Include(c => c.Jobs.Where(j => j.Status == "Active"))
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }
    }
}

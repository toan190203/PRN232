using Microsoft.EntityFrameworkCore;
using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public class JobRepository : Repository<Job>, IJobRepository
    {
        public JobRepository(PartTimeJobManagementContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Job>> GetAllAsync()
        {
            return await _dbSet
                .Include(j => j.Employer)
                .Include(j => j.Category)
                .Include(j => j.Applications)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();
        }

        public override async Task<Job?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(j => j.Employer)
                .Include(j => j.Category)
                .Include(j => j.Applications)
                .FirstOrDefaultAsync(j => j.JobId == id);
        }

        public async Task<Job?> GetJobWithDetailsAsync(int jobId)
        {
            return await _dbSet
                .Include(j => j.Employer)
                .Include(j => j.Category)
                .Include(j => j.Applications)
                    .ThenInclude(a => a.Student)
                .FirstOrDefaultAsync(j => j.JobId == jobId);
        }

        public async Task<IEnumerable<Job>> GetActiveJobsAsync()
        {
            return await _dbSet
                .Where(j => j.Status == "Open" && 
                           (j.ExpirationDate == null || j.ExpirationDate > DateTime.Now))
                .Include(j => j.Employer)
                .Include(j => j.Category)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetJobsByEmployerAsync(int employerId)
        {
            return await _dbSet
                .Where(j => j.EmployerId == employerId)
                .Include(j => j.Category)
                .Include(j => j.Applications)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetJobsByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(j => j.CategoryId == categoryId && j.Status == "Active")
                .Include(j => j.Employer)
                .Include(j => j.Category)
                .Include(j => j.Applications)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();
        }
    }
}

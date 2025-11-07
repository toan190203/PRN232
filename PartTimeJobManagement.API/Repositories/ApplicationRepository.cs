using Microsoft.EntityFrameworkCore;
using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public class ApplicationRepository : Repository<Application>, IApplicationRepository
    {
        public ApplicationRepository(PartTimeJobManagementContext context) : base(context)
        {
        }

        public async Task<Application?> GetApplicationWithDetailsAsync(int applicationId)
        {
            return await _dbSet
                .Include(a => a.Student)
                    .ThenInclude(s => s.StudentNavigation)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Employer)
                .Include(a => a.ApplicationHistories)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
        }

        public async Task<IEnumerable<Application>> GetApplicationsByStudentAsync(int studentId)
        {
            return await _dbSet
                .Where(a => a.StudentId == studentId)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Employer)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetApplicationsByJobAsync(int jobId)
        {
            return await _dbSet
                .Where(a => a.JobId == jobId)
                .Include(a => a.Student)
                    .ThenInclude(s => s.StudentNavigation)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetApplicationsByStatusAsync(string status)
        {
            return await _dbSet
                .Where(a => a.Status == status)
                .Include(a => a.Student)
                .Include(a => a.Job)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();
        }

        public async Task<bool> HasStudentAppliedToJobAsync(int studentId, int jobId)
        {
            return await _dbSet
                .AnyAsync(a => a.StudentId == studentId && a.JobId == jobId);
        }
    }
}

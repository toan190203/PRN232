using Microsoft.EntityFrameworkCore;
using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public class EmployerRepository : Repository<Employer>, IEmployerRepository
    {
        public EmployerRepository(PartTimeJobManagementContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Employer>> GetAllAsync()
        {
            return await _dbSet
                .Include(e => e.EmployerNavigation)
                    .ThenInclude(u => u.Role)
                .Include(e => e.Jobs)
                .ToListAsync();
        }

        public override async Task<Employer?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(e => e.EmployerNavigation)
                    .ThenInclude(u => u.Role)
                .Include(e => e.Jobs)
                .FirstOrDefaultAsync(e => e.EmployerId == id);
        }

        public async Task<Employer?> GetEmployerWithJobsAsync(int employerId)
        {
            return await _dbSet
                .Include(e => e.Jobs)
                    .ThenInclude(j => j.Applications)
                .Include(e => e.EmployerNavigation)
                    .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(e => e.EmployerId == employerId);
        }

        public async Task<IEnumerable<Employer>> GetVerifiedEmployersAsync()
        {
            return await _dbSet
                .Where(e => e.IsVerified)
                .ToListAsync();
        }

        public async Task<Employer?> GetEmployerByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(e => e.EmployerNavigation)
                .FirstOrDefaultAsync(e => e.EmployerId == userId);
        }
    }
}

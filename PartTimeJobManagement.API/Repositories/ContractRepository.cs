using Microsoft.EntityFrameworkCore;
using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public class ContractRepository : Repository<Contract>, IContractRepository
    {
        public ContractRepository(PartTimeJobManagementContext context) : base(context)
        {
        }

        public async Task<Contract?> GetContractWithDetailsAsync(int contractId)
        {
            return await _dbSet
                .Include(c => c.Application)
                    .ThenInclude(a => a.Student)
                .Include(c => c.Application)
                    .ThenInclude(a => a.Job)
                        .ThenInclude(j => j.Employer)
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.ContractId == contractId);
        }

        public async Task<Contract?> GetContractByApplicationIdAsync(int applicationId)
        {
            return await _dbSet
                .Include(c => c.Application)
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.ApplicationId == applicationId);
        }

        public async Task<IEnumerable<Contract>> GetActiveContractsAsync()
        {
            return await _dbSet
                .Where(c => c.Status == "Active")
                .Include(c => c.Application)
                    .ThenInclude(a => a.Student)
                .Include(c => c.Application)
                    .ThenInclude(a => a.Job)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetContractsByStatusAsync(string status)
        {
            return await _dbSet
                .Where(c => c.Status == status)
                .Include(c => c.Application)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetContractsByStudentIdAsync(int studentId)
        {
            return await _dbSet
                .Where(c => c.Application.StudentId == studentId)
                .Include(c => c.Application)
                    .ThenInclude(a => a.Student)
                .Include(c => c.Application)
                    .ThenInclude(a => a.Job)
                        .ThenInclude(j => j.Employer)
                .Include(c => c.Payments)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetContractsByEmployerIdAsync(int employerId)
        {
            return await _dbSet
                .Where(c => c.Application.Job.EmployerId == employerId)
                .Include(c => c.Application)
                    .ThenInclude(a => a.Student)
                .Include(c => c.Application)
                    .ThenInclude(a => a.Job)
                        .ThenInclude(j => j.Employer)
                .Include(c => c.Payments)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(PartTimeJobManagementContext context) : base(context)
        {
        }

        public async Task<Payment?> GetPaymentWithDetailsAsync(int paymentId)
        {
            return await _dbSet
                .Include(p => p.Contract)
                    .ThenInclude(c => c.Application)
                        .ThenInclude(a => a.Student)
                .Include(p => p.Contract)
                    .ThenInclude(c => c.Application)
                        .ThenInclude(a => a.Job)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByContractAsync(int contractId)
        {
            return await _dbSet
                .Where(p => p.ContractId == contractId)
                .Include(p => p.Contract)
                    .ThenInclude(c => c.Application)
                        .ThenInclude(a => a.Student)
                .Include(p => p.Contract)
                    .ThenInclude(c => c.Application)
                        .ThenInclude(a => a.Job)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStudentAsync(int studentId)
        {
            return await _dbSet
                .Where(p => p.Contract.Application.StudentId == studentId)
                .Include(p => p.Contract)
                    .ThenInclude(c => c.Application)
                        .ThenInclude(a => a.Student)
                .Include(p => p.Contract)
                    .ThenInclude(c => c.Application)
                        .ThenInclude(a => a.Job)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status)
        {
            return await _dbSet
                .Where(p => p.Status == status)
                .Include(p => p.Contract)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPaymentsByContractAsync(int contractId)
        {
            return await _dbSet
                .Where(p => p.ContractId == contractId && p.Status == "Completed")
                .SumAsync(p => p.Amount);
        }
    }
}

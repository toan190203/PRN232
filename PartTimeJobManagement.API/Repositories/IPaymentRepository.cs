using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment?> GetPaymentWithDetailsAsync(int paymentId);
        Task<IEnumerable<Payment>> GetPaymentsByContractAsync(int contractId);
        Task<IEnumerable<Payment>> GetPaymentsByStudentAsync(int studentId);
        Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status);
        Task<decimal> GetTotalPaymentsByContractAsync(int contractId);
    }
}

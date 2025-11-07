using PartTimeJobManagement.API.DTOs;

namespace PartTimeJobManagement.API.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentResponseDTO>> GetAllPaymentsAsync();
        Task<PaymentResponseDTO?> GetPaymentByIdAsync(int id);
        Task<PaymentResponseDTO> CreatePaymentAsync(CreatePaymentDTO dto);
        Task<PaymentResponseDTO> UpdatePaymentAsync(int id, UpdatePaymentDTO dto);
        Task<bool> DeletePaymentAsync(int id);
        Task<IEnumerable<PaymentResponseDTO>> GetPaymentsByContractAsync(int contractId);
        Task<IEnumerable<PaymentResponseDTO>> GetPaymentsByStudentAsync(int studentId);
        Task<IEnumerable<PaymentResponseDTO>> GetPaymentsByStatusAsync(string status);
    }
}

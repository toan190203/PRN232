using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Models;
using PartTimeJobManagement.API.Repositories;

namespace PartTimeJobManagement.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<PaymentResponseDTO>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            return payments.Select(MapToResponseDTO);
        }

        public async Task<PaymentResponseDTO?> GetPaymentByIdAsync(int id)
        {
            var payment = await _paymentRepository.GetPaymentWithDetailsAsync(id);
            return payment != null ? MapToResponseDTO(payment) : null;
        }

        public async Task<PaymentResponseDTO> CreatePaymentAsync(CreatePaymentDTO dto)
        {
            var payment = new Payment
            {
                ContractId = dto.ContractId,
                Amount = dto.Amount,
                PaymentDate = DateTime.Now,
                PaymentMethod = dto.PaymentMethod,
                Status = "Pending",
                Description = dto.Description
            };

            var created = await _paymentRepository.AddAsync(payment);
            return await GetPaymentByIdAsync(created.PaymentId) 
                ?? throw new Exception("Failed to retrieve created payment");
        }

        public async Task<PaymentResponseDTO> UpdatePaymentAsync(int id, UpdatePaymentDTO dto)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {id} not found");
            }

            if (dto.Amount.HasValue)
                payment.Amount = dto.Amount.Value;

            if (!string.IsNullOrEmpty(dto.PaymentMethod))
                payment.PaymentMethod = dto.PaymentMethod;

            if (!string.IsNullOrEmpty(dto.Status))
                payment.Status = dto.Status;

            if (!string.IsNullOrEmpty(dto.Description))
                payment.Description = dto.Description;

            await _paymentRepository.UpdateAsync(payment);
            return await GetPaymentByIdAsync(id) 
                ?? throw new Exception("Failed to retrieve updated payment");
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                return false;
            }

            await _paymentRepository.DeleteAsync(payment);
            return true;
        }

        public async Task<IEnumerable<PaymentResponseDTO>> GetPaymentsByContractAsync(int contractId)
        {
            var payments = await _paymentRepository.GetPaymentsByContractAsync(contractId);
            return payments.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<PaymentResponseDTO>> GetPaymentsByStudentAsync(int studentId)
        {
            var payments = await _paymentRepository.GetPaymentsByStudentAsync(studentId);
            return payments.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<PaymentResponseDTO>> GetPaymentsByStatusAsync(string status)
        {
            var payments = await _paymentRepository.GetPaymentsByStatusAsync(status);
            return payments.Select(MapToResponseDTO);
        }

        private PaymentResponseDTO MapToResponseDTO(Payment payment)
        {
            return new PaymentResponseDTO
            {
                PaymentId = payment.PaymentId,
                ContractId = payment.ContractId,
                StudentName = payment.Contract?.Application?.Student?.FullName ?? "",
                JobTitle = payment.Contract?.Application?.Job?.Title ?? "",
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                Description = payment.Description
            };
        }
    }
}

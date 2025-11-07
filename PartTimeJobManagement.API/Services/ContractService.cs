using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Models;
using PartTimeJobManagement.API.Repositories;

namespace PartTimeJobManagement.API.Services
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IPaymentRepository _paymentRepository;

        public ContractService(IContractRepository contractRepository, IPaymentRepository paymentRepository)
        {
            _contractRepository = contractRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<ContractResponseDTO>> GetAllContractsAsync()
        {
            var contracts = await _contractRepository.GetAllAsync();
            var dtos = new List<ContractResponseDTO>();

            foreach (var contract in contracts)
            {
                dtos.Add(await MapToResponseDTOAsync(contract));
            }

            return dtos;
        }

        public async Task<ContractResponseDTO?> GetContractByIdAsync(int id)
        {
            var contract = await _contractRepository.GetContractWithDetailsAsync(id);
            return contract != null ? await MapToResponseDTOAsync(contract) : null;
        }

        public async Task<ContractResponseDTO?> GetContractByApplicationIdAsync(int applicationId)
        {
            var contract = await _contractRepository.GetContractByApplicationIdAsync(applicationId);
            return contract != null ? await MapToResponseDTOAsync(contract) : null;
        }

        public async Task<ContractResponseDTO> CreateContractAsync(CreateContractDTO dto)
        {
            // Kiểm tra xem application đã có contract chưa
            var existing = await _contractRepository.GetContractByApplicationIdAsync(dto.ApplicationId);
            if (existing != null)
            {
                throw new InvalidOperationException("Contract already exists for this application");
            }

            var contract = new Contract
            {
                ApplicationId = dto.ApplicationId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                SalaryAgreed = dto.SalaryAgreed,
                ContractFile = dto.ContractFile,
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            var created = await _contractRepository.AddAsync(contract);
            return await GetContractByIdAsync(created.ContractId) 
                ?? throw new Exception("Failed to retrieve created contract");
        }

        public async Task<ContractResponseDTO> UpdateContractAsync(int id, UpdateContractDTO dto)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                throw new KeyNotFoundException($"Contract with ID {id} not found");
            }

            if (dto.EndDate.HasValue)
                contract.EndDate = dto.EndDate.Value;

            if (dto.SalaryAgreed.HasValue)
                contract.SalaryAgreed = dto.SalaryAgreed.Value;

            if (!string.IsNullOrEmpty(dto.ContractFile))
                contract.ContractFile = dto.ContractFile;

            if (!string.IsNullOrEmpty(dto.Status))
                contract.Status = dto.Status;

            await _contractRepository.UpdateAsync(contract);
            return await GetContractByIdAsync(id) 
                ?? throw new Exception("Failed to retrieve updated contract");
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return false;
            }

            await _contractRepository.DeleteAsync(contract);
            return true;
        }

        public async Task<IEnumerable<ContractResponseDTO>> GetActiveContractsAsync()
        {
            var contracts = await _contractRepository.GetActiveContractsAsync();
            var dtos = new List<ContractResponseDTO>();

            foreach (var contract in contracts)
            {
                dtos.Add(await MapToResponseDTOAsync(contract));
            }

            return dtos;
        }

        public async Task<IEnumerable<ContractResponseDTO>> GetContractsByStudentIdAsync(int studentId)
        {
            var contracts = await _contractRepository.GetContractsByStudentIdAsync(studentId);
            var dtos = new List<ContractResponseDTO>();

            foreach (var contract in contracts)
            {
                dtos.Add(await MapToResponseDTOAsync(contract));
            }

            return dtos;
        }

        public async Task<IEnumerable<ContractResponseDTO>> GetContractsByEmployerIdAsync(int employerId)
        {
            var contracts = await _contractRepository.GetContractsByEmployerIdAsync(employerId);
            var dtos = new List<ContractResponseDTO>();

            foreach (var contract in contracts)
            {
                dtos.Add(await MapToResponseDTOAsync(contract));
            }

            return dtos;
        }

        private async Task<ContractResponseDTO> MapToResponseDTOAsync(Contract contract)
        {
            var totalPaid = await _paymentRepository.GetTotalPaymentsByContractAsync(contract.ContractId);

            return new ContractResponseDTO
            {
                ContractId = contract.ContractId,
                ApplicationId = contract.ApplicationId,
                StudentId = contract.Application?.StudentId ?? 0,
                StudentName = contract.Application?.Student?.FullName ?? "",
                JobId = contract.Application?.JobId ?? 0,
                JobTitle = contract.Application?.Job?.Title ?? "",
                EmployerName = contract.Application?.Job?.Employer?.CompanyName ?? "",
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                SalaryAgreed = contract.SalaryAgreed,
                ContractFile = contract.ContractFile,
                Status = contract.Status,
                CreatedAt = contract.CreatedAt,
                TotalPayments = contract.Payments?.Count ?? 0,
                TotalPaid = totalPaid
            };
        }
    }
}

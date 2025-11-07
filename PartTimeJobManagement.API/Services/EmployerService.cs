using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Models;
using PartTimeJobManagement.API.Repositories;

namespace PartTimeJobManagement.API.Services
{
    public class EmployerService : IEmployerService
    {
        private readonly IEmployerRepository _employerRepository;
        private readonly IUserRepository _userRepository;

        public EmployerService(IEmployerRepository employerRepository, IUserRepository userRepository)
        {
            _employerRepository = employerRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<EmployerResponseDTO>> GetAllEmployersAsync()
        {
            var employers = await _employerRepository.GetAllAsync();
            return employers.Select(MapToResponseDTO);
        }

        public async Task<EmployerResponseDTO?> GetEmployerByIdAsync(int id)
        {
            var employer = await _employerRepository.GetEmployerWithJobsAsync(id);
            return employer != null ? MapToResponseDTO(employer) : null;
        }

        public async Task<EmployerResponseDTO?> GetEmployerByUserIdAsync(int userId)
        {
            var employer = await _employerRepository.GetEmployerByUserIdAsync(userId);
            return employer != null ? MapToResponseDTO(employer) : null;
        }

        public async Task<EmployerResponseDTO> CreateEmployerAsync(CreateEmployerDTO dto)
        {
            // Kiểm tra email đã tồn tại
            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Tạo User
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = 3, // Assuming 3 is Employer role ID
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var createdUser = await _userRepository.AddAsync(user);

            // Tạo Employer
            var employer = new Employer
            {
                EmployerId = createdUser.UserId,
                CompanyName = dto.CompanyName,
                ContactName = dto.ContactName,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                TaxCode = dto.TaxCode,
                IsVerified = false
            };

            var createdEmployer = await _employerRepository.AddAsync(employer);
            createdEmployer.EmployerNavigation = createdUser;

            return MapToResponseDTO(createdEmployer);
        }

        public async Task<EmployerResponseDTO> UpdateEmployerAsync(int id, UpdateEmployerDTO dto)
        {
            var employer = await _employerRepository.GetByIdAsync(id);
            if (employer == null)
            {
                throw new KeyNotFoundException($"Employer with ID {id} not found");
            }

            employer.CompanyName = dto.CompanyName;
            employer.ContactName = dto.ContactName;
            employer.PhoneNumber = dto.PhoneNumber;
            employer.Address = dto.Address;
            employer.TaxCode = dto.TaxCode;
            employer.IsVerified = dto.IsVerified;

            await _employerRepository.UpdateAsync(employer);

            return await GetEmployerByIdAsync(id) ?? throw new Exception("Failed to retrieve updated employer");
        }

        public async Task<bool> DeleteEmployerAsync(int id)
        {
            var employer = await _employerRepository.GetByIdAsync(id);
            if (employer == null)
            {
                return false;
            }

            await _employerRepository.DeleteAsync(employer);
            return true;
        }

        public async Task<IEnumerable<EmployerResponseDTO>> GetVerifiedEmployersAsync()
        {
            var employers = await _employerRepository.GetVerifiedEmployersAsync();
            return employers.Select(MapToResponseDTO);
        }

        private EmployerResponseDTO MapToResponseDTO(Employer employer)
        {
            return new EmployerResponseDTO
            {
                EmployerId = employer.EmployerId,
                Email = employer.EmployerNavigation?.Email ?? "",
                CompanyName = employer.CompanyName,
                ContactName = employer.ContactName,
                PhoneNumber = employer.PhoneNumber,
                Address = employer.Address,
                TaxCode = employer.TaxCode,
                IsVerified = employer.IsVerified,
                IsActive = employer.EmployerNavigation?.IsActive ?? false,
                CreatedAt = employer.EmployerNavigation?.CreatedAt ?? DateTime.MinValue,
                TotalJobs = employer.Jobs?.Count ?? 0
            };
        }
    }
}

using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Models;
using PartTimeJobManagement.API.Repositories;

namespace PartTimeJobManagement.API.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationService(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<IEnumerable<ApplicationResponseDTO>> GetAllApplicationsAsync()
        {
            var applications = await _applicationRepository.GetAllAsync();
            return applications.Select(MapToResponseDTO);
        }

        public async Task<ApplicationResponseDTO?> GetApplicationByIdAsync(int id)
        {
            var application = await _applicationRepository.GetApplicationWithDetailsAsync(id);
            return application != null ? MapToResponseDTO(application) : null;
        }

        public async Task<ApplicationResponseDTO> CreateApplicationAsync(CreateApplicationDTO dto)
        {
            // Kiểm tra đã apply chưa
            if (await _applicationRepository.HasStudentAppliedToJobAsync(dto.StudentId, dto.JobId))
            {
                throw new InvalidOperationException("Student has already applied to this job");
            }

            var application = new Application
            {
                StudentId = dto.StudentId,
                JobId = dto.JobId,
                ApplicationDate = DateTime.Now,
                CoverLetter = dto.CoverLetter,
                Status = "Pending"
            };

            var createdApplication = await _applicationRepository.AddAsync(application);
            return await GetApplicationByIdAsync(createdApplication.ApplicationId) 
                ?? throw new Exception("Failed to retrieve created application");
        }

        public async Task<ApplicationResponseDTO> UpdateApplicationStatusAsync(int id, string status)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
            {
                throw new KeyNotFoundException($"Application with ID {id} not found");
            }

            application.Status = status;
            await _applicationRepository.UpdateAsync(application);

            return await GetApplicationByIdAsync(id) 
                ?? throw new Exception("Failed to retrieve updated application");
        }

        public async Task<bool> DeleteApplicationAsync(int id)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
            {
                return false;
            }

            await _applicationRepository.DeleteAsync(application);
            return true;
        }

        public async Task<IEnumerable<ApplicationResponseDTO>> GetApplicationsByStudentAsync(int studentId)
        {
            var applications = await _applicationRepository.GetApplicationsByStudentAsync(studentId);
            return applications.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<ApplicationResponseDTO>> GetApplicationsByJobAsync(int jobId)
        {
            var applications = await _applicationRepository.GetApplicationsByJobAsync(jobId);
            return applications.Select(MapToResponseDTO);
        }

        private ApplicationResponseDTO MapToResponseDTO(Application application)
        {
            return new ApplicationResponseDTO
            {
                ApplicationId = application.ApplicationId,
                StudentId = application.StudentId,
                StudentName = application.Student?.FullName ?? "",
                JobId = application.JobId,
                JobTitle = application.Job?.Title ?? "",
                EmployerName = application.Job?.Employer?.CompanyName ?? "",
                ApplicationDate = application.ApplicationDate,
                CoverLetter = application.CoverLetter,
                Status = application.Status
            };
        }
    }
}

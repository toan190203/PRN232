using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Models;
using PartTimeJobManagement.API.Repositories;

namespace PartTimeJobManagement.API.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IApplicationRepository _applicationRepository;

        public JobService(IJobRepository jobRepository, IApplicationRepository applicationRepository)
        {
            _jobRepository = jobRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task<IEnumerable<JobResponseDTO>> GetAllJobsAsync()
        {
            var jobs = await _jobRepository.GetAllAsync();
            return jobs.Select(MapToResponseDTO);
        }

        public async Task<JobResponseDTO?> GetJobByIdAsync(int id)
        {
            // Thử lấy kèm navigation trước; nếu không có, fallback Find theo khóa chính
            var job = await _jobRepository.GetJobWithDetailsAsync(id)
                      ?? await _jobRepository.GetByIdAsync(id);
            return job != null ? MapToResponseDTO(job) : null;
        }

        public async Task<JobResponseDTO> CreateJobAsync(CreateJobDTO dto)
        {
            var job = new Job
            {
                EmployerId = dto.EmployerId,
                Title = dto.Title,
                Description = dto.Description,
                Salary = dto.Salary,
                Location = dto.Location,
                CategoryId = dto.CategoryId,
                PostedDate = DateTime.Now,
                ExpirationDate = dto.ExpirationDate,
                Status = "Open"
            };

            var createdJob = await _jobRepository.AddAsync(job);
            return await GetJobByIdAsync(createdJob.JobId) ?? throw new Exception("Failed to retrieve created job");
        }

        public async Task<JobResponseDTO> UpdateJobAsync(int id, UpdateJobDTO dto)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                throw new KeyNotFoundException($"Job with ID {id} not found");
            }

            job.Title = dto.Title;
            job.Description = dto.Description;
            job.Salary = dto.Salary;
            job.Location = dto.Location;
            job.CategoryId = dto.CategoryId;
            job.ExpirationDate = dto.ExpirationDate;
            job.Status = dto.Status;

            await _jobRepository.UpdateAsync(job);
            return await GetJobByIdAsync(id) ?? throw new Exception("Failed to retrieve updated job");
        }

        public async Task<bool> DeleteJobAsync(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return false;
            }

            // Kiểm tra xem job có applications không
            var applications = await _applicationRepository.GetApplicationsByJobAsync(id);
            if (applications != null && applications.Any())
            {
                throw new InvalidOperationException($"Cannot delete job. There are {applications.Count()} application(s) for this job.");
            }

            await _jobRepository.DeleteAsync(job);
            return true;
        }

        public async Task<IEnumerable<JobResponseDTO>> GetActiveJobsAsync()
        {
            var jobs = await _jobRepository.GetActiveJobsAsync();
            return jobs.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<JobResponseDTO>> GetJobsByEmployerAsync(int employerId)
        {
            var jobs = await _jobRepository.GetJobsByEmployerAsync(employerId);
            return jobs.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<JobResponseDTO>> GetJobsByCategoryAsync(int categoryId)
        {
            var jobs = await _jobRepository.GetJobsByCategoryAsync(categoryId);
            return jobs.Select(MapToResponseDTO);
        }

        private JobResponseDTO MapToResponseDTO(Job job)
        {
            return new JobResponseDTO
            {
                JobId = job.JobId,
                EmployerId = job.EmployerId,
                EmployerName = job.Employer?.CompanyName ?? "",
                Title = job.Title,
                Description = job.Description,
                Salary = job.Salary,
                Location = job.Location,
                CategoryId = job.CategoryId,
                CategoryName = job.Category?.CategoryName ?? "",
                PostedDate = job.PostedDate,
                ExpirationDate = job.ExpirationDate,
                Status = job.Status,
                TotalApplications = job.Applications?.Count ?? 0
            };
        }
    }
}

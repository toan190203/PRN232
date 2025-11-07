using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Models;
using PartTimeJobManagement.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace PartTimeJobManagement.API.Services
{
    public class ApplicationHistoryService : IApplicationHistoryService
    {
        private readonly IApplicationHistoryRepository _historyRepository;
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationHistoryService(
            IApplicationHistoryRepository historyRepository,
            IApplicationRepository applicationRepository)
        {
            _historyRepository = historyRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task<IEnumerable<ApplicationHistoryResponseDTO>> GetHistoryByApplicationIdAsync(int applicationId)
        {
            var histories = await _historyRepository.GetHistoryByApplicationIdAsync(applicationId);
            var application = await _applicationRepository.GetApplicationWithDetailsAsync(applicationId);

            return histories.Select(h => MapToResponseDTO(h, application));
        }

        public async Task<ApplicationHistoryResponseDTO?> GetHistoryByIdAsync(int id)
        {
            var history = await _historyRepository.GetByIdAsync(id);
            if (history == null)
                return null;

            var application = await _applicationRepository.GetApplicationWithDetailsAsync(history.ApplicationId);
            return MapToResponseDTO(history, application);
        }

        public async Task<ApplicationHistoryResponseDTO> CreateHistoryAsync(CreateApplicationHistoryDTO dto)
        {
            var application = await _applicationRepository.GetByIdAsync(dto.ApplicationId);
            if (application == null)
            {
                throw new KeyNotFoundException($"Application with ID {dto.ApplicationId} not found");
            }

            var history = new ApplicationHistory
            {
                ApplicationId = dto.ApplicationId,
                Status = dto.Status,
                ChangedAt = DateTime.Now
            };

            var created = await _historyRepository.AddAsync(history);
            return await GetHistoryByIdAsync(created.HistoryId) 
                ?? throw new Exception("Failed to retrieve created history");
        }

        private ApplicationHistoryResponseDTO MapToResponseDTO(ApplicationHistory history, Application? application)
        {
            return new ApplicationHistoryResponseDTO
            {
                HistoryId = history.HistoryId,
                ApplicationId = history.ApplicationId,
                StudentName = application?.Student?.FullName ?? "",
                JobTitle = application?.Job?.Title ?? "",
                Status = history.Status,
                ChangedAt = history.ChangedAt,
                Note = null
            };
        }
    }
}

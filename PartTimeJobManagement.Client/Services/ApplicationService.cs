using PartTimeJobManagement.Client.Models.DTOs;

namespace PartTimeJobManagement.Client.Services
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationResponseDTO>?> GetAllApplicationsAsync();
        Task<IEnumerable<ApplicationResponseDTO>?> GetAllApplicationsAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
        Task<IEnumerable<ApplicationResponseDTO>?> GetApplicationsByStudentAsync(int studentId);
        Task<IEnumerable<ApplicationResponseDTO>?> GetApplicationsByStudentAsync(int studentId, string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
        Task<IEnumerable<ApplicationResponseDTO>?> GetApplicationsByJobAsync(int jobId);
        Task<IEnumerable<ApplicationResponseDTO>?> GetApplicationsByJobAsync(int jobId, string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
        Task<ApplicationResponseDTO?> GetApplicationByIdAsync(int id);
        Task<ApplicationResponseDTO?> CreateApplicationAsync(CreateApplicationDTO applicationDto);
        Task<bool> DeleteApplicationAsync(int id);
        Task<bool> UpdateApplicationStatusAsync(int id, string status);
    }

    public class ApplicationService : BaseApiService, IApplicationService
    {
        public ApplicationService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) 
            : base(httpClient, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<ApplicationResponseDTO>?> GetAllApplicationsAsync()
        {
            return await GetAsync<IEnumerable<ApplicationResponseDTO>>("api/Applications");
        }

        public async Task<IEnumerable<ApplicationResponseDTO>?> GetAllApplicationsAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
        {
            var queryParams = BuildODataQuery(filter, orderBy, top, skip);
            return await GetAsync<IEnumerable<ApplicationResponseDTO>>($"api/Applications{queryParams}");
        }

        public async Task<IEnumerable<ApplicationResponseDTO>?> GetApplicationsByStudentAsync(int studentId)
        {
            return await GetAsync<IEnumerable<ApplicationResponseDTO>>($"api/Applications/student/{studentId}");
        }

        public async Task<IEnumerable<ApplicationResponseDTO>?> GetApplicationsByStudentAsync(int studentId, string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
        {
            var queryParams = BuildODataQuery(filter, orderBy, top, skip);
            return await GetAsync<IEnumerable<ApplicationResponseDTO>>($"api/Applications/student/{studentId}{queryParams}");
        }

        public async Task<IEnumerable<ApplicationResponseDTO>?> GetApplicationsByJobAsync(int jobId)
        {
            return await GetAsync<IEnumerable<ApplicationResponseDTO>>($"api/Applications/job/{jobId}");
        }

        public async Task<IEnumerable<ApplicationResponseDTO>?> GetApplicationsByJobAsync(int jobId, string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
        {
            var queryParams = BuildODataQuery(filter, orderBy, top, skip);
            return await GetAsync<IEnumerable<ApplicationResponseDTO>>($"api/Applications/job/{jobId}{queryParams}");
        }

        public async Task<ApplicationResponseDTO?> GetApplicationByIdAsync(int id)
        {
            return await GetAsync<ApplicationResponseDTO>($"api/Applications/{id}");
        }

        public async Task<ApplicationResponseDTO?> CreateApplicationAsync(CreateApplicationDTO applicationDto)
        {
            return await PostAsync<CreateApplicationDTO, ApplicationResponseDTO>("api/Applications", applicationDto);
        }

        public async Task<bool> DeleteApplicationAsync(int id)
        {
            return await DeleteAsync($"api/Applications/{id}");
        }

        public async Task<bool> UpdateApplicationStatusAsync(int id, string status)
        {
            try
            {
                SetAuthorizationHeader();
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"api/Applications/{id}/status")
                {
                    Content = JsonContent.Create(new { Status = status })
                };

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

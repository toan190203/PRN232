using PartTimeJobManagement.Client.Models.DTOs;
using System.Net.Http.Json;

namespace PartTimeJobManagement.Client.Services
{
    public interface IEmployerService
    {
        Task<List<EmployerResponseDTO>> GetAllEmployersAsync();
        Task<List<EmployerResponseDTO>> GetAllEmployersAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
        Task<EmployerResponseDTO?> GetEmployerByIdAsync(int id);
        Task<EmployerResponseDTO?> GetEmployerByUserIdAsync(int userId);
        Task<bool> UpdateEmployerAsync(int id, UpdateEmployerDTO employer);
        Task<bool> DeleteEmployerAsync(int id);
    }

    public class EmployerService : BaseApiService, IEmployerService
    {
        public EmployerService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor)
        {
        }

        public async Task<List<EmployerResponseDTO>> GetAllEmployersAsync()
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("api/Employers");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<EmployerResponseDTO>>() ?? new List<EmployerResponseDTO>();
                }
                return new List<EmployerResponseDTO>();
            }
            catch (Exception)
            {
                return new List<EmployerResponseDTO>();
            }
        }

        public async Task<List<EmployerResponseDTO>> GetAllEmployersAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
        {
            try
            {
                SetAuthorizationHeader();
                var queryParams = BuildODataQuery(filter, orderBy, top, skip);
                var response = await _httpClient.GetAsync($"api/Employers{queryParams}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<EmployerResponseDTO>>() ?? new List<EmployerResponseDTO>();
                }
                return new List<EmployerResponseDTO>();
            }
            catch (Exception)
            {
                return new List<EmployerResponseDTO>();
            }
        }

        public async Task<EmployerResponseDTO?> GetEmployerByIdAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/Employers/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<EmployerResponseDTO>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<EmployerResponseDTO?> GetEmployerByUserIdAsync(int userId)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/Employers/user/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<EmployerResponseDTO>();
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateEmployerAsync(int id, UpdateEmployerDTO employer)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.PutAsJsonAsync($"api/Employers/{id}", employer);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteEmployerAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"api/Employers/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

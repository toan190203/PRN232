using PartTimeJobManagement.Client.Models.DTOs;
using System.Net.Http.Json;

namespace PartTimeJobManagement.Client.Services
{
    public interface IJobCategoryService
    {
        Task<List<JobCategoryResponseDTO>?> GetAllCategoriesAsync();
        Task<JobCategoryResponseDTO?> GetCategoryByIdAsync(int id);
    }

    public class JobCategoryService : BaseApiService, IJobCategoryService
    {
        public JobCategoryService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor)
        {
        }

        public async Task<List<JobCategoryResponseDTO>?> GetAllCategoriesAsync()
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("api/JobCategories");
                if (response.IsSuccessStatusCode)
                {
                    var categories = await response.Content.ReadFromJsonAsync<List<JobCategoryResponseDTO>>();
                    return categories;
                }
                
                return new List<JobCategoryResponseDTO>();
            }
            catch (Exception)
            {
                return new List<JobCategoryResponseDTO>();
            }
        }

        public async Task<JobCategoryResponseDTO?> GetCategoryByIdAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/JobCategories/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<JobCategoryResponseDTO>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

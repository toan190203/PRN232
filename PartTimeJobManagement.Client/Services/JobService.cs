using PartTimeJobManagement.Client.Models.DTOs;

namespace PartTimeJobManagement.Client.Services
{
    public interface IJobService
    {
        Task<IEnumerable<JobResponseDTO>?> GetAllJobsAsync();
        Task<IEnumerable<JobResponseDTO>?> GetAllJobsAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
        Task<IEnumerable<JobResponseDTO>?> GetActiveJobsAsync();
        Task<IEnumerable<JobResponseDTO>?> GetActiveJobsAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
        Task<JobResponseDTO?> GetJobByIdAsync(int id);
        Task<IEnumerable<JobResponseDTO>?> GetJobsByEmployerAsync(int employerId);
        Task<IEnumerable<JobResponseDTO>?> GetJobsByEmployerAsync(int employerId, string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
        Task<IEnumerable<JobResponseDTO>?> GetJobsByCategoryAsync(int categoryId);
        Task<IEnumerable<JobResponseDTO>?> GetJobsByCategoryAsync(int categoryId, string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
        Task<JobResponseDTO?> CreateJobAsync(CreateJobDTO jobDto);
        Task<JobResponseDTO?> UpdateJobAsync(int id, UpdateJobDTO jobDto);
        Task<bool> DeleteJobAsync(int id);
    }

    public class JobService : BaseApiService, IJobService
    {
        public JobService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) 
            : base(httpClient, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<JobResponseDTO>?> GetAllJobsAsync()
        {
            return await GetAsync<IEnumerable<JobResponseDTO>>("api/Jobs");
        }

        public async Task<IEnumerable<JobResponseDTO>?> GetAllJobsAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
        {
            var queryParams = BuildODataQuery(filter, orderBy, top, skip);
            return await GetAsync<IEnumerable<JobResponseDTO>>($"api/Jobs{queryParams}");
        }

        public async Task<IEnumerable<JobResponseDTO>?> GetActiveJobsAsync()
        {
            return await GetAsync<IEnumerable<JobResponseDTO>>("api/Jobs/active");
        }

        public async Task<IEnumerable<JobResponseDTO>?> GetActiveJobsAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
        {
            var queryParams = BuildODataQuery(filter, orderBy, top, skip);
            return await GetAsync<IEnumerable<JobResponseDTO>>($"api/Jobs/active{queryParams}");
        }

        public async Task<JobResponseDTO?> GetJobByIdAsync(int id)
        {
            return await GetAsync<JobResponseDTO>($"api/Jobs/{id}");
        }

        public async Task<IEnumerable<JobResponseDTO>?> GetJobsByEmployerAsync(int employerId)
        {
            return await GetAsync<IEnumerable<JobResponseDTO>>($"api/Jobs/employer/{employerId}");
        }

        public async Task<IEnumerable<JobResponseDTO>?> GetJobsByEmployerAsync(int employerId, string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
        {
            var queryParams = BuildODataQuery(filter, orderBy, top, skip);
            return await GetAsync<IEnumerable<JobResponseDTO>>($"api/Jobs/employer/{employerId}{queryParams}");
        }

        public async Task<IEnumerable<JobResponseDTO>?> GetJobsByCategoryAsync(int categoryId)
        {
            return await GetAsync<IEnumerable<JobResponseDTO>>($"api/Jobs/category/{categoryId}");
        }

        public async Task<IEnumerable<JobResponseDTO>?> GetJobsByCategoryAsync(int categoryId, string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
        {
            var queryParams = BuildODataQuery(filter, orderBy, top, skip);
            return await GetAsync<IEnumerable<JobResponseDTO>>($"api/Jobs/category/{categoryId}{queryParams}");
        }

        public async Task<JobResponseDTO?> CreateJobAsync(CreateJobDTO jobDto)
        {
            return await PostAsync<CreateJobDTO, JobResponseDTO>("api/Jobs", jobDto);
        }

        public async Task<JobResponseDTO?> UpdateJobAsync(int id, UpdateJobDTO jobDto)
        {
            return await PutAsync<UpdateJobDTO, JobResponseDTO>($"api/Jobs/{id}", jobDto);
        }

        public async Task<bool> DeleteJobAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"api/Jobs/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    // Parse error message từ API
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(errorContent);
                        if (errorObj != null && errorObj.ContainsKey("message"))
                        {
                            throw new InvalidOperationException(errorObj["message"].GetString() ?? "Cannot delete job.");
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        throw; // Re-throw InvalidOperationException
                    }
                    catch
                    {
                        throw new InvalidOperationException("Cannot delete job. Please try again.");
                    }
                }
                
                return false;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw để controller catch
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

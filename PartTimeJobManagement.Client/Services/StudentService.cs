using PartTimeJobManagement.Client.Models.DTOs;
using System.Net.Http.Json;

namespace PartTimeJobManagement.Client.Services
{
    public interface IStudentService
    {
        Task<List<StudentResponseDTO>> GetAllStudentsAsync();
        Task<List<StudentResponseDTO>> GetAllStudentsAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
        Task<StudentResponseDTO?> GetStudentByIdAsync(int id);
        Task<StudentResponseDTO?> GetStudentByUserIdAsync(int userId);
        Task<StudentResponseDTO?> CreateStudentAsync(CreateStudentProfileDTO student);
        Task<bool> UpdateStudentAsync(int id, UpdateStudentDTO student);
        Task<bool> DeleteStudentAsync(int id);
        Task<(bool success, string message)> UploadCVAsync(int studentId, IFormFile file);
    }

    public class StudentService : BaseApiService, IStudentService
    {
        public StudentService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor)
        {
        }

        public async Task<List<StudentResponseDTO>> GetAllStudentsAsync()
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("api/Students");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<StudentResponseDTO>>() ?? new List<StudentResponseDTO>();
                }
                return new List<StudentResponseDTO>();
            }
            catch (Exception)
            {
                return new List<StudentResponseDTO>();
            }
        }

        public async Task<List<StudentResponseDTO>> GetAllStudentsAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
        {
            try
            {
                SetAuthorizationHeader();
                var queryParams = BuildODataQuery(filter, orderBy, top, skip);
                var response = await _httpClient.GetAsync($"api/Students{queryParams}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<StudentResponseDTO>>() ?? new List<StudentResponseDTO>();
                }
                return new List<StudentResponseDTO>();
            }
            catch (Exception)
            {
                return new List<StudentResponseDTO>();
            }
        }

        public async Task<StudentResponseDTO?> GetStudentByIdAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/Students/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<StudentResponseDTO>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<StudentResponseDTO?> GetStudentByUserIdAsync(int userId)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/Students/user/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<StudentResponseDTO>();
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<StudentResponseDTO?> CreateStudentAsync(CreateStudentProfileDTO student)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync("api/Students/profile", student);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<StudentResponseDTO>();
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateStudentAsync(int id, UpdateStudentDTO student)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.PutAsJsonAsync($"api/Students/{id}", student);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"api/Students/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<(bool success, string message)> UploadCVAsync(int studentId, IFormFile file)
        {
            try
            {
                SetAuthorizationHeader();
                
                using var content = new MultipartFormDataContent();
                using var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.FileName);

                var response = await _httpClient.PostAsync($"api/Students/{studentId}/upload-cv", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, "CV uploaded successfully");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return (false, error);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}

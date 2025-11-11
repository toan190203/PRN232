using PartTimeJobManagement.Client.Models.DTOs;
using System.Net.Http.Headers;

namespace PartTimeJobManagement.Client.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
        Task<UserResponseDTO?> GetUserByIdAsync(int userId);
        Task<bool> UpdateUserAsync(int userId, UpdateUserDTO updateUserDto);
        Task<bool> DeleteUserAsync(int userId);
    }

    public class UserService : BaseApiService, IUserService
    {
        public UserService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("api/Users");

                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserResponseDTO>>();
                    return users ?? new List<UserResponseDTO>();
                }

                return new List<UserResponseDTO>();
            }
            catch (Exception)
            {
                return new List<UserResponseDTO>();
            }
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
        {
            try
            {
                SetAuthorizationHeader();
                var queryParams = BuildODataQuery(filter, orderBy, top, skip);
                var response = await _httpClient.GetAsync($"api/Users{queryParams}");

                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserResponseDTO>>();
                    return users ?? new List<UserResponseDTO>();
                }

                return new List<UserResponseDTO>();
            }
            catch (Exception)
            {
                return new List<UserResponseDTO>();
            }
        }

        public async Task<UserResponseDTO?> GetUserByIdAsync(int userId)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/Users/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserResponseDTO>();
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync(int userId, UpdateUserDTO updateUserDto)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.PutAsJsonAsync($"api/Users/{userId}", updateUserDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"api/Users/{userId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

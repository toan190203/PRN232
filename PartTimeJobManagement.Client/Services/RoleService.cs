using PartTimeJobManagement.Client.Models.DTOs;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PartTimeJobManagement.Client.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponseDTO>> GetAllRolesAsync();
        Task<IEnumerable<RoleResponseDTO>> GetAllRolesAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
        Task<RoleResponseDTO?> GetRoleByIdAsync(int roleId);
        Task<RoleResponseDTO?> GetRoleByNameAsync(string name);
        Task<RoleResponseDTO?> CreateRoleAsync(CreateRoleDTO createRoleDto);
        Task<bool> UpdateRoleAsync(int roleId, UpdateRoleDTO updateRoleDto);
        Task<bool> DeleteRoleAsync(int roleId);
    }

    public class RoleService : BaseApiService, IRoleService
    {
        public RoleService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<RoleResponseDTO>> GetAllRolesAsync()
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("api/Roles");

                if (response.IsSuccessStatusCode)
                {
                    var roles = await response.Content.ReadFromJsonAsync<IEnumerable<RoleResponseDTO>>();
                    return roles ?? new List<RoleResponseDTO>();
                }

                return new List<RoleResponseDTO>();
            }
            catch (Exception)
            {
                return new List<RoleResponseDTO>();
            }
        }

        public async Task<IEnumerable<RoleResponseDTO>> GetAllRolesAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
        {
            try
            {
                SetAuthorizationHeader();
                var queryParams = BuildODataQuery(filter, orderBy, top, skip);
                var response = await _httpClient.GetAsync($"api/Roles{queryParams}");

                if (response.IsSuccessStatusCode)
                {
                    var roles = await response.Content.ReadFromJsonAsync<IEnumerable<RoleResponseDTO>>();
                    return roles ?? new List<RoleResponseDTO>();
                }

                return new List<RoleResponseDTO>();
            }
            catch (Exception)
            {
                return new List<RoleResponseDTO>();
            }
        }

        public async Task<RoleResponseDTO?> GetRoleByIdAsync(int roleId)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/Roles/{roleId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RoleResponseDTO>();
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<RoleResponseDTO?> GetRoleByNameAsync(string name)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/Roles/name/{name}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RoleResponseDTO>();
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<RoleResponseDTO?> CreateRoleAsync(CreateRoleDTO createRoleDto)
        {
            try
            {
                SetAuthorizationHeader();
                var json = JsonSerializer.Serialize(createRoleDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/Roles", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RoleResponseDTO>();
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateRoleAsync(int roleId, UpdateRoleDTO updateRoleDto)
        {
            try
            {
                SetAuthorizationHeader();
                var json = JsonSerializer.Serialize(updateRoleDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/Roles/{roleId}", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"api/Roles/{roleId}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

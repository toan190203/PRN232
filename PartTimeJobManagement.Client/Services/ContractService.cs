using PartTimeJobManagement.Client.Models.DTOs;
using System.Net.Http.Json;

namespace PartTimeJobManagement.Client.Services
{
    public interface IContractService
    {
        Task<List<ContractResponseDTO>?> GetAllContractsAsync();
        Task<List<ContractResponseDTO>?> GetAllContractsAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
        Task<ContractResponseDTO?> GetContractByIdAsync(int id);
        Task<List<ContractResponseDTO>?> GetContractsByStudentAsync(int studentId);
        Task<List<ContractResponseDTO>?> GetContractsByStudentAsync(int studentId, string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
        Task<List<ContractResponseDTO>?> GetContractsByEmployerAsync(int employerId);
        Task<List<ContractResponseDTO>?> GetContractsByEmployerAsync(int employerId, string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
        Task<ContractResponseDTO?> CreateContractAsync(CreateContractDTO contract);
        Task<bool> UpdateContractAsync(int id, UpdateContractDTO contract);
        Task<bool> DeleteContractAsync(int id);
    }

    public class ContractService : BaseApiService, IContractService
    {
        public ContractService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor)
        {
        }

        public async Task<List<ContractResponseDTO>?> GetAllContractsAsync()
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("api/Contracts");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ContractResponseDTO>>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<ContractResponseDTO>?> GetAllContractsAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
        {
            try
            {
                SetAuthorizationHeader();
                var queryParams = BuildODataQuery(filter, orderBy, top, skip);
                var response = await _httpClient.GetAsync($"api/Contracts{queryParams}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ContractResponseDTO>>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ContractResponseDTO?> GetContractByIdAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/Contracts/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ContractResponseDTO>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<ContractResponseDTO>?> GetContractsByStudentAsync(int studentId)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/Contracts/student/{studentId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ContractResponseDTO>>();
                }
                
                return new List<ContractResponseDTO>();
            }
            catch (Exception)
            {
                return new List<ContractResponseDTO>();
            }
        }

        public async Task<List<ContractResponseDTO>?> GetContractsByStudentAsync(int studentId, string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
        {
            try
            {
                SetAuthorizationHeader();
                var queryParams = BuildODataQuery(filter, orderBy, top, skip);
                var response = await _httpClient.GetAsync($"api/Contracts/student/{studentId}{queryParams}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ContractResponseDTO>>();
                }
                
                return new List<ContractResponseDTO>();
            }
            catch (Exception)
            {
                return new List<ContractResponseDTO>();
            }
        }

        public async Task<List<ContractResponseDTO>?> GetContractsByEmployerAsync(int employerId)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/Contracts/employer/{employerId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ContractResponseDTO>>();
                }
                
                return new List<ContractResponseDTO>();
            }
            catch (Exception)
            {
                return new List<ContractResponseDTO>();
            }
        }

        public async Task<List<ContractResponseDTO>?> GetContractsByEmployerAsync(int employerId, string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
        {
            try
            {
                SetAuthorizationHeader();
                var queryParams = BuildODataQuery(filter, orderBy, top, skip);
                var response = await _httpClient.GetAsync($"api/Contracts/employer/{employerId}{queryParams}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ContractResponseDTO>>();
                }
                
                return new List<ContractResponseDTO>();
            }
            catch (Exception)
            {
                return new List<ContractResponseDTO>();
            }
        }

        public async Task<ContractResponseDTO?> CreateContractAsync(CreateContractDTO contract)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync("api/Contracts", contract);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ContractResponseDTO>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateContractAsync(int id, UpdateContractDTO contract)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.PutAsJsonAsync($"api/Contracts/{id}", contract);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"api/Contracts/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

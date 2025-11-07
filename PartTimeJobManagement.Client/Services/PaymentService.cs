using PartTimeJobManagement.Client.Models.DTOs;
using System.Net.Http.Json;

namespace PartTimeJobManagement.Client.Services
{
    public interface IPaymentService
    {
        Task<List<PaymentResponseDTO>?> GetAllPaymentsAsync();
        Task<PaymentResponseDTO?> GetPaymentByIdAsync(int id);
        Task<List<PaymentResponseDTO>?> GetPaymentsByContractAsync(int contractId);
        Task<List<PaymentResponseDTO>?> GetPaymentsByStudentAsync(int studentId);
        Task<PaymentResponseDTO?> CreatePaymentAsync(CreatePaymentDTO payment);
        Task<bool> UpdatePaymentAsync(int id, UpdatePaymentDTO payment);
        Task<bool> DeletePaymentAsync(int id);
    }

    public class PaymentService : BaseApiService, IPaymentService
    {
        public PaymentService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, httpContextAccessor)
        {
        }

        public async Task<List<PaymentResponseDTO>?> GetAllPaymentsAsync()
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("api/Payments");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<PaymentResponseDTO>>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<PaymentResponseDTO?> GetPaymentByIdAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/Payments/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<PaymentResponseDTO>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<PaymentResponseDTO>?> GetPaymentsByContractAsync(int contractId)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/Payments/contract/{contractId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<PaymentResponseDTO>>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<PaymentResponseDTO>?> GetPaymentsByStudentAsync(int studentId)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/Payments/student/{studentId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<PaymentResponseDTO>>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<PaymentResponseDTO?> CreatePaymentAsync(CreatePaymentDTO payment)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync("api/Payments", payment);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<PaymentResponseDTO>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdatePaymentAsync(int id, UpdatePaymentDTO payment)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.PutAsJsonAsync($"api/Payments/{id}", payment);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"api/Payments/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

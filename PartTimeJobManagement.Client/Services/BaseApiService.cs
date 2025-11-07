using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PartTimeJobManagement.Client.Services
{
    public class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly JsonSerializerOptions _jsonOptions;

        public BaseApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        protected void SetAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        protected async Task<T?> GetAsync<T>(string endpoint)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync(endpoint);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, _jsonOptions);
            }

            return default;
        }

        protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            SetAuthorizationHeader();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(endpoint, content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
            }

            return default;
        }

        protected async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            SetAuthorizationHeader();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync(endpoint, content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
            }

            return default;
        }

        protected async Task<bool> DeleteAsync(string endpoint)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }

        protected async Task<string?> GetErrorMessage(HttpResponseMessage response)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                var error = JsonSerializer.Deserialize<Dictionary<string, string>>(content, _jsonOptions);
                return error?.GetValueOrDefault("message");
            }
            catch
            {
                return response.ReasonPhrase;
            }
        }
    }
}

using PartTimeJobManagement.Client.Models.DTOs;

namespace PartTimeJobManagement.Client.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDto);
        Task<AuthResponseDTO?> RegisterAsync(RegisterDTO registerDto);
        Task<bool> LogoutAsync();
        Task<UserResponseDTO?> GetCurrentUserAsync();
        bool IsAuthenticated();
        string? GetCurrentUserRole();
        Task<(bool success, string message)> ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDto);
    }

    public class AuthService : BaseApiService, IAuthService
    {
        public AuthService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) 
            : base(httpClient, httpContextAccessor)
        {
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDto)
        {
            var response = await PostAsync<LoginDTO, AuthResponseDTO>("api/Auth/login", loginDto);
            
            if (response != null)
            {
                // Lưu token vào session
                _httpContextAccessor.HttpContext?.Session.SetString("JWTToken", response.Token);
                _httpContextAccessor.HttpContext?.Session.SetString("UserRole", response.Role);
                _httpContextAccessor.HttpContext?.Session.SetInt32("UserId", response.UserId);
                _httpContextAccessor.HttpContext?.Session.SetString("UserEmail", response.Email);
            }

            return response;
        }

        public async Task<AuthResponseDTO?> RegisterAsync(RegisterDTO registerDto)
        {
            var response = await PostAsync<RegisterDTO, AuthResponseDTO>("api/Auth/register", registerDto);
            
            if (response != null)
            {
                // Lưu token vào session
                _httpContextAccessor.HttpContext?.Session.SetString("JWTToken", response.Token);
                _httpContextAccessor.HttpContext?.Session.SetString("UserRole", response.Role);
                _httpContextAccessor.HttpContext?.Session.SetInt32("UserId", response.UserId);
                _httpContextAccessor.HttpContext?.Session.SetString("UserEmail", response.Email);
            }

            return response;
        }

        public Task<bool> LogoutAsync()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
            return Task.FromResult(true);
        }

        public async Task<UserResponseDTO?> GetCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (userId == null)
                return null;

            return await GetAsync<UserResponseDTO>($"api/Auth/user/{userId}");
        }

        public bool IsAuthenticated()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            return !string.IsNullOrEmpty(token);
        }

        public string? GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
        }

        public async Task<(bool success, string message)> ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDto)
        {
            try
            {
                SetAuthorizationHeader(); // Thêm JWT token vào header

                // Tạo DTO chỉ với CurrentPassword và NewPassword để gửi đến API
                var apiDto = new 
                { 
                    CurrentPassword = changePasswordDto.CurrentPassword,
                    NewPassword = changePasswordDto.NewPassword
                };

                var response = await _httpClient.PostAsJsonAsync($"api/Auth/change-password/{userId}", apiDto);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Password changed successfully");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    // Cố gắng parse JSON error message
                    try
                    {
                        var errorObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(errorContent);
                        if (errorObj != null && errorObj.ContainsKey("message"))
                        {
                            return (false, errorObj["message"].GetString() ?? "Current password is incorrect");
                        }
                    }
                    catch
                    {
                        // Nếu không parse được
                    }
                    
                    return (false, "Current password is incorrect");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return (false, "User not found");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    // Cố gắng parse JSON error message
                    try
                    {
                        var errorObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(errorContent);
                        if (errorObj != null && errorObj.ContainsKey("message"))
                        {
                            return (false, errorObj["message"].GetString() ?? "Failed to change password");
                        }
                    }
                    catch
                    {
                        // Nếu không parse được, trả về message mặc định
                    }
                    
                    return (false, "Failed to change password. Please try again.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
    }
}

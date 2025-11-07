using PartTimeJobManagement.API.DTOs;

namespace PartTimeJobManagement.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto);
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);
        Task<UserResponseDTO?> GetUserByIdAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDTO dto);
    }
}

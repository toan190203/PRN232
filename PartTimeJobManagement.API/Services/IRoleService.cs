using PartTimeJobManagement.API.DTOs;

namespace PartTimeJobManagement.API.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponseDTO>> GetAllRolesAsync();
        Task<RoleResponseDTO?> GetRoleByIdAsync(int id);
        Task<RoleResponseDTO?> GetRoleByNameAsync(string name);
        Task<RoleResponseDTO> CreateRoleAsync(CreateRoleDTO dto);
        Task<RoleResponseDTO> UpdateRoleAsync(int id, UpdateRoleDTO dto);
        Task<bool> DeleteRoleAsync(int id);
    }
}

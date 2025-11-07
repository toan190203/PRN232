using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Models;
using PartTimeJobManagement.API.Repositories;

namespace PartTimeJobManagement.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<RoleResponseDTO>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(MapToResponseDTO);
        }

        public async Task<RoleResponseDTO?> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            return role != null ? MapToResponseDTO(role) : null;
        }

        public async Task<RoleResponseDTO?> GetRoleByNameAsync(string name)
        {
            var role = await _roleRepository.GetRoleByNameAsync(name);
            return role != null ? MapToResponseDTO(role) : null;
        }

        public async Task<RoleResponseDTO> CreateRoleAsync(CreateRoleDTO dto)
        {
            // Kiểm tra role name đã tồn tại
            var existing = await _roleRepository.GetRoleByNameAsync(dto.RoleName);
            if (existing != null)
            {
                throw new InvalidOperationException("Role name already exists");
            }

            var role = new Role
            {
                RoleName = dto.RoleName
            };

            var created = await _roleRepository.AddAsync(role);
            return MapToResponseDTO(created);
        }

        public async Task<RoleResponseDTO> UpdateRoleAsync(int id, UpdateRoleDTO dto)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {id} not found");
            }

            role.RoleName = dto.RoleName;
            await _roleRepository.UpdateAsync(role);

            return await GetRoleByIdAsync(id) ?? throw new Exception("Failed to retrieve updated role");
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return false;
            }

            await _roleRepository.DeleteAsync(role);
            return true;
        }

        private RoleResponseDTO MapToResponseDTO(Role role)
        {
            return new RoleResponseDTO
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                TotalUsers = role.Users?.Count ?? 0
            };
        }
    }
}

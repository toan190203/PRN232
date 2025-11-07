using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role?> GetRoleByNameAsync(string roleName);
    }
}

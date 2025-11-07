using Microsoft.EntityFrameworkCore;
using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(PartTimeJobManagementContext context) : base(context)
        {
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.RoleName == roleName);
        }
    }
}

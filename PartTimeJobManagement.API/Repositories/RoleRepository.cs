using Microsoft.EntityFrameworkCore;
using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(PartTimeJobManagementContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _dbSet
                .Include(r => r.Users)
                .ToListAsync();
        }

        public override async Task<Role?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.RoleId == id);
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _dbSet
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.RoleName == roleName);
        }
    }
}

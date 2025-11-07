using Microsoft.EntityFrameworkCore;
using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(PartTimeJobManagementContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.Student)
                .Include(u => u.Employer)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserWithRoleAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }
    }
}

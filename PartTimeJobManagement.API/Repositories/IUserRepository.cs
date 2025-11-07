using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserWithRoleAsync(int userId);
        Task<bool> EmailExistsAsync(string email);
    }
}

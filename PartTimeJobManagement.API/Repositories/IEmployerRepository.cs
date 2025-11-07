using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public interface IEmployerRepository : IRepository<Employer>
    {
        Task<Employer?> GetEmployerWithJobsAsync(int employerId);
        Task<IEnumerable<Employer>> GetVerifiedEmployersAsync();
        Task<Employer?> GetEmployerByUserIdAsync(int userId);
    }
}

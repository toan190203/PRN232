using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public interface IJobRepository : IRepository<Job>
    {
        Task<Job?> GetJobWithDetailsAsync(int jobId);
        Task<IEnumerable<Job>> GetActiveJobsAsync();
        Task<IEnumerable<Job>> GetJobsByEmployerAsync(int employerId);
        Task<IEnumerable<Job>> GetJobsByCategoryAsync(int categoryId);
    }
}

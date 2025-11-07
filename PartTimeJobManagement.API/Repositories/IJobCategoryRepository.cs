using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public interface IJobCategoryRepository : IRepository<JobCategory>
    {
        Task<JobCategory?> GetCategoryByNameAsync(string name);
        Task<IEnumerable<JobCategory>> GetCategoriesWithJobsAsync();
    }
}

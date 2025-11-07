using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public interface IApplicationRepository : IRepository<Application>
    {
        Task<Application?> GetApplicationWithDetailsAsync(int applicationId);
        Task<IEnumerable<Application>> GetApplicationsByStudentAsync(int studentId);
        Task<IEnumerable<Application>> GetApplicationsByJobAsync(int jobId);
        Task<IEnumerable<Application>> GetApplicationsByStatusAsync(string status);
        Task<bool> HasStudentAppliedToJobAsync(int studentId, int jobId);
    }
}

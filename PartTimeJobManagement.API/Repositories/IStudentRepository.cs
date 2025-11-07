using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student?> GetStudentWithApplicationsAsync(int studentId);
        Task<IEnumerable<Student>> GetStudentsByMajorAsync(string major);
        Task<Student?> GetStudentByUserIdAsync(int userId);
    }
}

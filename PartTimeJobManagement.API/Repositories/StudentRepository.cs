using Microsoft.EntityFrameworkCore;
using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(PartTimeJobManagementContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _dbSet
                .Include(s => s.StudentNavigation)
                    .ThenInclude(u => u.Role)
                .Include(s => s.Applications)
                .ToListAsync();
        }

        public override async Task<Student?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.StudentNavigation)
                    .ThenInclude(u => u.Role)
                .Include(s => s.Applications)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<Student?> GetStudentWithApplicationsAsync(int studentId)
        {
            return await _dbSet
                .Include(s => s.Applications)
                    .ThenInclude(a => a.Job)
                .Include(s => s.StudentNavigation)
                    .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }

        public async Task<IEnumerable<Student>> GetStudentsByMajorAsync(string major)
        {
            return await _dbSet
                .Where(s => s.Major == major)
                .ToListAsync();
        }

        public async Task<Student?> GetStudentByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(s => s.StudentNavigation)
                    .ThenInclude(u => u.Role)
                .Include(s => s.Applications)
                .FirstOrDefaultAsync(s => s.StudentId == userId);
        }
    }
}

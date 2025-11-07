using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Models;

namespace PartTimeJobManagement.API.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentResponseDTO>> GetAllStudentsAsync();
        Task<StudentResponseDTO?> GetStudentByIdAsync(int id);
        Task<StudentResponseDTO?> GetStudentByUserIdAsync(int userId);
        Task<StudentResponseDTO> CreateStudentAsync(CreateStudentDTO dto);
        Task<StudentResponseDTO> CreateStudentProfileAsync(CreateStudentProfileDTO dto);
        Task<StudentResponseDTO> UpdateStudentAsync(int id, UpdateStudentDTO dto);
        Task<bool> DeleteStudentAsync(int id);
        Task<IEnumerable<StudentResponseDTO>> GetStudentsByMajorAsync(string major);
    }
}

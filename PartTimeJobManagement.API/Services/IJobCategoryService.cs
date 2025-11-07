using PartTimeJobManagement.API.DTOs;

namespace PartTimeJobManagement.API.Services
{
    public interface IJobCategoryService
    {
        Task<IEnumerable<JobCategoryResponseDTO>> GetAllCategoriesAsync();
        Task<JobCategoryResponseDTO?> GetCategoryByIdAsync(int id);
        Task<JobCategoryResponseDTO> CreateCategoryAsync(CreateJobCategoryDTO dto);
        Task<JobCategoryResponseDTO> UpdateCategoryAsync(int id, UpdateJobCategoryDTO dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}

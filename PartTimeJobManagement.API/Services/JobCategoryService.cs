using PartTimeJobManagement.API.DTOs;
using PartTimeJobManagement.API.Models;
using PartTimeJobManagement.API.Repositories;

namespace PartTimeJobManagement.API.Services
{
    public class JobCategoryService : IJobCategoryService
    {
        private readonly IJobCategoryRepository _categoryRepository;

        public JobCategoryService(IJobCategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<JobCategoryResponseDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(MapToResponseDTO);
        }

        public async Task<JobCategoryResponseDTO?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category != null ? MapToResponseDTO(category) : null;
        }

        public async Task<JobCategoryResponseDTO> CreateCategoryAsync(CreateJobCategoryDTO dto)
        {
            // Kiểm tra tên category đã tồn tại
            var existing = await _categoryRepository.GetCategoryByNameAsync(dto.CategoryName);
            if (existing != null)
            {
                throw new InvalidOperationException("Category name already exists");
            }

            var category = new JobCategory
            {
                CategoryName = dto.CategoryName
            };

            var created = await _categoryRepository.AddAsync(category);
            return MapToResponseDTO(created);
        }

        public async Task<JobCategoryResponseDTO> UpdateCategoryAsync(int id, UpdateJobCategoryDTO dto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found");
            }

            category.CategoryName = dto.CategoryName;
            await _categoryRepository.UpdateAsync(category);

            return await GetCategoryByIdAsync(id) ?? throw new Exception("Failed to retrieve updated category");
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return false;
            }

            await _categoryRepository.DeleteAsync(category);
            return true;
        }

        private JobCategoryResponseDTO MapToResponseDTO(JobCategory category)
        {
            return new JobCategoryResponseDTO
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                TotalJobs = category.Jobs?.Count ?? 0
            };
        }
    }
}

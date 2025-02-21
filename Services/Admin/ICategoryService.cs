using AshishGeneralStore.DTOs.Inventory;

namespace AshishGeneralStore.Services.Admin
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CategoryCreateUpdateDto categoryDto);
        Task<bool> UpdateCategoryAsync(int id, CategoryCreateUpdateDto categoryDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}

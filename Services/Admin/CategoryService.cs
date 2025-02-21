using AshishGeneralStore.Data;
using AshishGeneralStore.DTOs.Inventory;
using AshishGeneralStore.Models;
using Microsoft.EntityFrameworkCore;

namespace AshishGeneralStore.Services.Admin
{
    public class CategoryService : ICategoryService
    {
        private readonly StoreDbContext _context;

        public CategoryService(StoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            return category;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryCreateUpdateDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryCreateUpdateDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }

            category.Name = categoryDto.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }

            if (await _context.Products.AnyAsync(p => p.CategoryId == id))
            {
                throw new InvalidOperationException("Cannot delete category with associated products.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

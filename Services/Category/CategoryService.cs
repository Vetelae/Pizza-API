using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Category;

namespace Pizza_API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _dbContext;
        
        public CategoryService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _dbContext.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);

            if (category == null)
                return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
                return null;

            // Update the entity
            category.Name = dto.Name;

            // Save changes
            await _dbContext.SaveChangesAsync();

            // Return updated Dto
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
                return false;

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

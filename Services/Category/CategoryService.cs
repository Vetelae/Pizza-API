using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Category;

namespace Pizza_API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IImageService _imageService;

        public CategoryService(ApplicationDbContext dbContext, IImageService imageService)
        {
            _dbContext = dbContext;
            _imageService = imageService;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _dbContext.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImagePath = c.ImagePath,
                    ImageFileName = c.ImageFileName,
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
                Name = category.Name,
                ImagePath = category.ImagePath,
                ImageFileName = category.ImageFileName
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                ImagePath = "/uploads/default/defaultCategory.png",
                ImageFileName = "defaultCategory.png"
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ImagePath = category.ImagePath,
                ImageFileName = category.ImageFileName
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
                Name = category.Name,
                ImagePath = category.ImagePath,
                ImageFileName = category.ImageFileName
            };
        }

        public async Task<string?> UploadCategoryImageAsync(IFormFile file, int categoryId)
        {
            var category = await _dbContext.Categories.FindAsync(categoryId);
            if (category == null)
                return null;

            // Delete old image if it's not the default
            if (category.ImageFileName != "defaultCategory.png")
                _imageService.DeleteImage(category.ImageFileName, "categories");

            var (imagePath, imageFileName) = await _imageService.UploadImageAsync(file, categoryId.ToString(), "categories");

            category.ImagePath = imagePath;
            category.ImageFileName = imageFileName;
            await _dbContext.SaveChangesAsync();

            return imagePath;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
                return false;

            // Delete image only if it's not the default
            if (category.ImageFileName != "defaultCategory.png")
                _imageService.DeleteImage(category.ImageFileName, "categories");

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
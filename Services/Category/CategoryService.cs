using Microsoft.EntityFrameworkCore;
using Pizza_API.Constants;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Category;
using Pizza_API.Exceptions;

namespace Pizza_API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IImageService _imageService;
        private readonly IOutputCacheInvalidator _outputCacheInvalidator;

        public CategoryService(
            ApplicationDbContext dbContext,
            IImageService imageService,
            IOutputCacheInvalidator outputCacheInvalidator)
        {
            _dbContext = dbContext;
            _imageService = imageService;
            _outputCacheInvalidator = outputCacheInvalidator;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImagePath = c.ImagePath,
                    ImageFileName = c.ImageFileName,
                })
                .ToListAsync();
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _dbContext.Categories
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImagePath = c.ImagePath,
                    ImageFileName = c.ImageFileName
                })
                .SingleOrDefaultAsync();

            if (category == null)
                throw new NotFoundException($"Category {id} not found");

            return category;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            // Trim name to avoid whitespace
            var normalizedName = dto.Name.Trim();

            // Check if category name exists
            var nameExists = await _dbContext.Categories
                .AnyAsync(c => c.Name.ToLower() == normalizedName.ToLower());

            if (nameExists)
                throw new ConflictException($"A category with the name '{normalizedName}' already exists.");

            var category = new Category
            {
                Name = normalizedName,
                ImagePath = "/uploads/default/defaultCategory.png",
                ImageFileName = "defaultCategory.png"
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
            await _outputCacheInvalidator.EvictByTagAsync(OutputCacheTags.Categories);

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ImagePath = category.ImagePath,
                ImageFileName = category.ImageFileName
            };
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
                throw new NotFoundException($"Category {id} not found");

            // Trim name to avoid whitespace
            var normalizedName = dto.Name.Trim();

            // Check if category name exists
            var nameExists = await _dbContext.Categories
                .AnyAsync(c => c.Id != id && c.Name.ToLower() == normalizedName.ToLower());

            if (nameExists)
                throw new ConflictException($"A category with the name '{normalizedName}' already exists.");

            // Update the entity
            category.Name = normalizedName;

            // Save changes
            await _dbContext.SaveChangesAsync();
            await _outputCacheInvalidator.EvictByTagAsync(OutputCacheTags.Categories);

            // Return updated Dto
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ImagePath = category.ImagePath,
                ImageFileName = category.ImageFileName
            };
        }

        public async Task<string> UploadCategoryImageAsync(IFormFile file, int categoryId)
        {
            var category = await _dbContext.Categories.FindAsync(categoryId);
            if (category == null)
                throw new NotFoundException($"Category {categoryId} not found");

            var (imagePath, imageFileName) = await _imageService.UploadImageAsync(file, categoryId.ToString(), "categories");

            // Delete old image if it's not the default
            if (category.ImageFileName != "defaultCategory.png")
                _imageService.DeleteImage(category.ImageFileName, "categories");

            category.ImagePath = imagePath;
            category.ImageFileName = imageFileName;
            await _dbContext.SaveChangesAsync();
            await _outputCacheInvalidator.EvictByTagAsync(OutputCacheTags.Categories);

            return imagePath;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);

            if (category == null)
                throw new NotFoundException($"Category {id} not found");

            var hasMenuItems = await _dbContext.MenuItems.AnyAsync(m => m.CategoryId == id);

            if (hasMenuItems)
                throw new ConflictException($"Cannot delete category {id} because it has associated menu items.");

            // Delete image only if it's not the default
            if (category.ImageFileName != "defaultCategory.png")
                _imageService.DeleteImage(category.ImageFileName, "categories");

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
            await _outputCacheInvalidator.EvictByTagAsync(OutputCacheTags.Categories);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Pizza_API.Constants;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.MenuItem;
using Pizza_API.Exceptions;

namespace Pizza_API.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IImageService _imageService;
        private readonly IOutputCacheInvalidator _outputCacheInvalidator;

        public MenuItemService(
            ApplicationDbContext dbContext,
            IImageService imageService,
            IOutputCacheInvalidator outputCacheInvalidator)
        {
            _dbContext = dbContext;
            _imageService = imageService;
            _outputCacheInvalidator = outputCacheInvalidator;
        }
        public async Task<List<MenuItemDto>> GetAllMenuItemsAsync()
        {
            return await _dbContext.MenuItems
                .AsNoTracking()
                .Select(m => new MenuItemDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    IsAvailable = m.IsAvailable,
                    CategoryId = m.CategoryId,
                    ImagePath = m.ImagePath
                })
                .ToListAsync();
        }

        public async Task<MenuItemDto> GetMenuItemByIdAsync(int id)
        {
            var menuItem = await _dbContext.MenuItems
                .AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new MenuItemDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    IsAvailable = m.IsAvailable,
                    CategoryId = m.CategoryId,
                    ImagePath = m.ImagePath
                })
                .SingleOrDefaultAsync();

            if (menuItem == null)
                throw new NotFoundException($"MenuItem {id} not found");

            return menuItem;
        }

        public async Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto)
        {
            var category = await _dbContext.Categories.FindAsync(dto.CategoryId);
            if (category == null)
                throw new NotFoundException($"Category {dto.CategoryId} not found");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("MenuItem name is required.");

            var menuItem = new MenuItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                IsAvailable = dto.IsAvailable,
                CategoryId = dto.CategoryId,
                ImagePath = "/uploads/default/defaultMenuItem.png",
                ImageFileName = "defaultMenuItem.png"
            };

            _dbContext.MenuItems.Add(menuItem);
            await _dbContext.SaveChangesAsync();
            await _outputCacheInvalidator.EvictByTagAsync(OutputCacheTags.MenuItems);

            return new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                CategoryId = menuItem.CategoryId,
                ImagePath = menuItem.ImagePath
            };
        }

        public async Task<MenuItemDto> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto)
        {
            var menuItem = await _dbContext.MenuItems
                .Include(m => m.Category)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
                throw new NotFoundException($"MenuItem {id} not found");

            var category = await _dbContext.Categories.FindAsync(dto.CategoryId);
            if (category == null)
                throw new NotFoundException($"Category {dto.CategoryId} not found");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("MenuItem name is required.");

            // Update the entity
            menuItem.Name = dto.Name;
            menuItem.Description = dto.Description;
            menuItem.Price = dto.Price;
            menuItem.IsAvailable = dto.IsAvailable;
            menuItem.CategoryId = dto.CategoryId;

            // Save changes
            await _dbContext.SaveChangesAsync();
            await _outputCacheInvalidator.EvictByTagAsync(OutputCacheTags.MenuItems);

            // Return updated DTO
            return new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                CategoryId = menuItem.CategoryId,
                ImagePath = menuItem.ImagePath
            };
        }

        public async Task<string> UploadMenuItemImageAsync(IFormFile file, int menuItemId)
        {
            var menuItem = await _dbContext.MenuItems.FindAsync(menuItemId);
            if (menuItem == null)
                throw new NotFoundException($"MenuItem {menuItemId} not found");

            var oldImageFileName = menuItem.ImageFileName;

            // Upload new image
            var (imagePath, imageFileName) = await _imageService.UploadImageAsync(file, menuItemId.ToString(), "menu-items");

            // Update MenuItem in database
            menuItem.ImagePath = imagePath;
            menuItem.ImageFileName = imageFileName;
            await _dbContext.SaveChangesAsync();
            await _outputCacheInvalidator.EvictByTagAsync(OutputCacheTags.MenuItems);

            // Only delete old image after DB save succeeds
            _imageService.DeleteImage(oldImageFileName, "menu-items");

            return imagePath;
        }

        public async Task DeleteMenuItemAsync(int id)
        {
            var menuItem = await _dbContext.MenuItems
                .Include(m => m.Category)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
                throw new NotFoundException($"MenuItem {id} not found");

            _dbContext.MenuItems.Remove(menuItem);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new ConflictException($"MenuItem {id} cannot be deleted because it is referenced elsewhere.");
            }

            await _outputCacheInvalidator.EvictByTagAsync(OutputCacheTags.MenuItems);

            // Delete image file only after the DB delete succeeds
            _imageService.DeleteImage(menuItem.ImageFileName, "menu-items");
        }
    }
}

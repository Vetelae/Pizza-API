using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.MenuItem;

namespace Pizza_API.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IImageService _imageService;

        public MenuItemService(ApplicationDbContext dbContext, IImageService imageService)
        {
            _dbContext = dbContext;
            _imageService = imageService;
        }
        public async Task<List<MenuItemDto>> GetAllMenuItemsAsync()
        {
            var menuItems = await _dbContext.MenuItems
                .Include(m => m.Category)
                .ToListAsync();

            return menuItems.Select(m => new MenuItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Price = m.Price,
                IsAvailable = m.IsAvailable,
                CategoryId = m.CategoryId,
                ImagePath = m.ImagePath
            }).ToList();
        }

        public async Task<MenuItemDto?> GetMenuItemByIdAsync(int id)
        {
            var menuItem = await _dbContext.MenuItems
                .Include(m => m.Category)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
                return null;

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

        public async Task<MenuItemDto?> CreateMenuItemAsync(CreateMenuItemDto dto)
        {
            var category = await _dbContext.Categories.FindAsync(dto.CategoryId);
            if (category == null)
                return null;

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

            return new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                CategoryId = menuItem.CategoryId
            };
        }

        public async Task<MenuItemDto?> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto)
        {
            var menuItem = await _dbContext.MenuItems
                .Include(m => m.Category)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
                return null;

            // Update the entity
            menuItem.Name = dto.Name;
            menuItem.Description = dto.Description;
            menuItem.Price = dto.Price;
            menuItem.IsAvailable = dto.IsAvailable;
            menuItem.CategoryId = dto.CategoryId;

            // Save changes
            await _dbContext.SaveChangesAsync();

            // Return updated DTO
            return new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                CategoryId = menuItem.CategoryId
            };
        }

        public async Task<string?> UploadMenuItemImageAsync(IFormFile file, int menuItemId)
        {
            var menuItem = await _dbContext.MenuItems.FindAsync(menuItemId);
            if (menuItem == null)
                return null;

            // Upload new image
            var (imagePath, imageFileName) = await _imageService.UploadImageAsync(file, menuItemId.ToString(), "menu-items");

            // Delete old image if exists
            _imageService.DeleteImage(menuItem.ImageFileName, "menu-items");

            // Update MenuItem in database
            menuItem.ImagePath = imagePath;
            menuItem.ImageFileName = imageFileName;
            await _dbContext.SaveChangesAsync();

            return imagePath;
        }

        public async Task<bool> DeleteMenuItemAsync(int id)
        {
            var menuItem = await _dbContext.MenuItems
                .Include(m => m.Category)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
                return false;

            try
            {
                // Delete image file if it exists
                _imageService.DeleteImage(menuItem.ImageFileName, "menu-items");

                // Delete from database
                _dbContext.MenuItems.Remove(menuItem);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting menu item: {ex.Message}");
                return false;
            }
        }
    }
}
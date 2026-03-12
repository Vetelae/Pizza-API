using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.MenuItem;
using SendGrid.Helpers.Errors.Model;
using SendGrid.Helpers.Mail;


namespace Pizza_API.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/menu-items");

        public MenuItemService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
                    CategoryId  = m.CategoryId,
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
                Description= menuItem.Description,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                CategoryId = menuItem.CategoryId,
                ImagePath = menuItem.ImagePath
            };
        }

        public async Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto)
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

        public async Task <MenuItemDto?> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto)
        {
            var menuItem = await _dbContext.MenuItems
                .Include(m =>  m.Category)
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

        public async Task<string> UploadMenuItemImageAsync(IFormFile file, int menuItemId)
        {
            // Validate file existence
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType))
                throw new ArgumentException("Invalid file type. Allowed types: JPEG, PNG, WebP");

            // Validate file size (max 5MB)
            const long maxFileSize = 5 * 1024 * 1024;
            if (file.Length > maxFileSize)
                throw new ArgumentException("File too large. Maximum size is 5MB");

            // Check if MenuItem exists
            var menuItem = await _dbContext.MenuItems.FindAsync(menuItemId);
            if (menuItem == null)
                throw new NotFoundException("MenuItem not found");

            try
            {
                // Create uploads folder if it doesn't exist
                Directory.CreateDirectory(_uploadsFolder);

                // Delete old image if it exists
                if (!string.IsNullOrEmpty(menuItem.ImageFileName))
                {
                    var oldFilePath = Path.Combine(_uploadsFolder, menuItem.ImageFileName);
                    if (File.Exists(oldFilePath))
                    {
                        try
                        {
                            File.Delete(oldFilePath);
                        }
                        catch (IOException ex)
                        {
                            // Log but don't fail - old file will just stay
                            Console.WriteLine($"Could not delete old image: {ex.Message}");
                        }
                    }
                }

                // Generate unique filename (sanitize original filename)
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{menuItemId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(_uploadsFolder, fileName);

                // Save file to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Update MenuItem in database
                menuItem.ImagePath = $"/uploads/menu-items/{fileName}";
                menuItem.ImageFileName = fileName;
                await _dbContext.SaveChangesAsync();

                return menuItem.ImagePath;
            }
            catch (IOException ex)
            {
                throw new Exception($"Error saving file to disk: {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error saving to database: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error during image upload: {ex.Message}");
            }
        }

        public async Task <bool> DeleteMenuItemAsync(int id)
        {
            var menuItem = await _dbContext.MenuItems
                .Include(m => m.Category)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (menuItem == null) 
                return false;

            try
            {
                // Delete image file if it exists
                if (!string.IsNullOrEmpty(menuItem.ImageFileName))
                {
                    var imagePath = Path.Combine(_uploadsFolder, menuItem.ImageFileName);
                    if (File.Exists(imagePath))
                    {
                        try
                        {
                            File.Delete(imagePath);
                        }
                        catch (IOException ex)
                        {
                            // Log but don't fail - database deletion will still succeed
                            Console.WriteLine($"Could not delete image file: {ex.Message}");
                        }
                    }
                }

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
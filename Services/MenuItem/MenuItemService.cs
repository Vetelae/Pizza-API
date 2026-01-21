using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Pizza;


namespace Pizza_API.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly ApplicationDbContext _dbContext;
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
                    CategoryId  = m.CategoryId
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

        public async Task <bool> DeleteMenuItemAsync(int id)
        {
            var menuItem = await _dbContext.MenuItems
                .Include(m => m.Category)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (menuItem == null) 
                return false;

            _dbContext.MenuItems.Remove(menuItem);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
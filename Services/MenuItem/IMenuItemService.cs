using Pizza_API.Entities.Dtos.MenuItem;

namespace Pizza_API.Services
{
    public interface IMenuItemService
    {
        Task<List<MenuItemDto>> GetAllMenuItemsAsync();
        Task<MenuItemDto?> GetMenuItemByIdAsync(int id);
        Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto);
        Task<MenuItemDto?> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto);
        Task<bool> DeleteMenuItemAsync(int id);
        Task<string?> UploadMenuItemImageAsync(IFormFile file, int menuItemId);
    }
}
using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.MenuItem;
using Pizza_API.Services;

namespace Pizza_API.Controllers
{
    [Route("api/public/menuitems")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        // GET: List of all MenuItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetAllMenuItems() 
        {
            var menuItems = await _menuItemService.GetAllMenuItemsAsync();

            return Ok(menuItems);
        }

        // GET: MenuItem by id
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItemDto>> GetMenuItemById(int id) 
        {
            var menuItem = await _menuItemService.GetMenuItemByIdAsync(id);
            if (menuItem == null)
                return NotFound();

            return Ok(menuItem);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.MenuItem;
using Pizza_API.Services;

namespace Pizza_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        public MenuController(IMenuItemService menuItemService)
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

        // POST: Create new MenuItem
        [HttpPost]
        public async Task<ActionResult<MenuItemDto>> CreateMenuItem(CreateMenuItemDto dto)
        {
            var createdMenuItem = await _menuItemService.CreateMenuItemAsync(dto);

            if (createdMenuItem == null)
                return BadRequest();

            return CreatedAtAction(nameof(GetMenuItemById),
                new { id = createdMenuItem.Id },
                createdMenuItem);
        }

        // PUT: Update existing MenuItem
        [HttpPut("{id}")]
        public async Task<ActionResult<MenuItemDto>> UpdateMenuItem(int id, UpdateMenuItemDto dto)
        {
            var updated = await _menuItemService.UpdateMenuItemAsync(id, dto);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // DELETE: Delete existing MenuItem
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var deleted = await _menuItemService.DeleteMenuItemAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
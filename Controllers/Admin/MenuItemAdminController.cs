using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.MenuItem;
using Pizza_API.Services;

namespace Pizza_API.Controllers
{
    [Route("api/admin/menuitems")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class MenuItemAdminController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemAdminController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        // POST: Create new MenuItem
        [HttpPost]
        public async Task<ActionResult<MenuItemDto>> CreateMenuItem(CreateMenuItemDto dto)
        {
            var createdMenuItem = await _menuItemService.CreateMenuItemAsync(dto);

            if (createdMenuItem == null)
                return BadRequest();

            return CreatedAtAction(
                actionName: "GetMenuItemById",
                controllerName: "MenuItem",
                routeValues: new { id = createdMenuItem.Id },
                value: createdMenuItem
            );
        }

        // POST: Upload MenuItem image
        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            try
            {
                var imagePath = await _menuItemService.UploadMenuItemImageAsync(file, id);
                return Ok(new { imagePath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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
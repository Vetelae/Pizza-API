using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.Category;
using Pizza_API.Services;

namespace Pizza_API.Controllers
{
    [Route("api/admin/categories")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoryAdminController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryAdminController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // POST: Create new category
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto dto)
        {
            var createdCategory = await _categoryService.CreateCategoryAsync(dto);

            return CreatedAtAction(
                actionName: "GetCategoryById",
                controllerName: "Category",
                routeValues: new { id = createdCategory.Id },
                value: createdCategory
            );
        }

        // PUT: Update existing category
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, UpdateCategoryDto dto)
        {
            var updated = await _categoryService.UpdateCategoryAsync(id, dto);
            return Ok(updated);
        }

        // POST: Upload category image
        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadCategoryImage(int id, IFormFile file)
        {
            var imagePath = await _categoryService.UploadCategoryImageAsync(file, id);
            return Ok(new { imagePath });
        }

        // DELETE: Delete existing category
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);

            return NoContent();
        }
    }
}
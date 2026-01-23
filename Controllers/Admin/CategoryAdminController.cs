using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.Category;
using Pizza_API.Services;

namespace Pizza_API.Controllers
{
    [Route("api/admin/categories")]
    [ApiController]
    public class CategoryAdminController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryAdminController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: List of all categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            return Ok(categories);
        }

        // GET: Category by id
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        // POST: Create new category
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto dto)
        {
            var createdCategory = await _categoryService.CreateCategoryAsync(dto);

            if (createdCategory == null)
                return BadRequest();

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

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // DELETE: Delete existing category
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
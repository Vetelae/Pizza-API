using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using Pizza_API.Constants;
using Pizza_API.Entities.Dtos.Category;
using Pizza_API.Services;

namespace Pizza_API.Controllers
{
    [Route("api/public/categories")]
    [ApiController]
    [EnableRateLimiting(RateLimitPolicies.PublicReads)]
    [OutputCache(PolicyName = OutputCachePolicies.PublicReads, Tags = new[] { OutputCacheTags.Categories })]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
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
            return Ok(category);
        }
    }
}

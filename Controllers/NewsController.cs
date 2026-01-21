using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.News;
using Pizza_API.Services;

namespace Pizza_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        // GET: List of all news
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsDto>>> GetAllNews()
        {
            var news = await _newsService.GetAllNewsAsync();

            return Ok(news);
        }

        // GET: News by id
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsDto>> GetNewsById(int id)
        {
            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null)
                return NotFound();

            return Ok(news);
        }

        // POST: Create new news
        [HttpPost]
        public async Task<ActionResult<NewsDto>> CreateNews(CreateNewsDto dto)
        {
            var createdNews = await _newsService.CreateNewsAsync(dto);

            if (createdNews == null)
                return BadRequest();

            return CreatedAtAction(nameof(GetNewsById),
                new { id = createdNews.Id },
                createdNews);
        }

        // PUT: Update existing news
        [HttpPut("{id}")]
        public async Task<ActionResult<NewsDto>> UpdateNews(int id, UpdateNewsDto dto)
        {
            var updated = await _newsService.UpdateNewsAsync(id, dto);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // DELETE: Delete existing news
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var deleted = await _newsService.DeleteNewsAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

    }
}

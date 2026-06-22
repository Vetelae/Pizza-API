using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.News;
using Pizza_API.Services;

namespace Pizza_API.Controllers
{
    [Route("api/admin/news")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class NewsAdminController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsAdminController(INewsService newsService)
        {
            _newsService = newsService;
        }

        // POST: Create new news
        [HttpPost]
        public async Task<ActionResult<NewsDto>> CreateNews(CreateNewsDto dto)
        {
            var createdNews = await _newsService.CreateNewsAsync(dto);

            return CreatedAtAction(
                actionName: "GetNewsById",
                controllerName: "News",
                routeValues: new { id = createdNews.Id },
                value: createdNews
            );
        }

        // PUT: Update existing news
        [HttpPut("{id}")]
        public async Task<ActionResult<NewsDto>> UpdateNews(int id, UpdateNewsDto dto)
        {
            var updated = await _newsService.UpdateNewsAsync(id, dto);

            return Ok(updated);
        }

        // DELETE: Delete existing news
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            await _newsService.DeleteNewsAsync(id);

            return NoContent();
        }
    }
}
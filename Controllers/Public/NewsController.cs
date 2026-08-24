using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using Pizza_API.Constants;
using Pizza_API.Entities.Dtos.News;
using Pizza_API.Services;

namespace Pizza_API.Controllers
{
    [Route("api/public/news")]
    [ApiController]
    [EnableRateLimiting(RateLimitPolicies.PublicReads)]
    [OutputCache(PolicyName = OutputCachePolicies.PublicReads, Tags = new[] { OutputCacheTags.News })]
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
            return Ok(news);
        }
    }
}

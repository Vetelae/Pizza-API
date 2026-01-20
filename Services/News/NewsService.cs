using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities.Dtos.News;

namespace Pizza_API.Services
{
    public class NewsService : INewsService
    {
        private readonly ApplicationDbContext _dbContext;
        
        public NewsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext; 
        }

        public async Task<List<NewsDto>> GetAllNewsAsync()
        {
            return await _dbContext.News
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    Date = n.Date,
                    Title = n.Title,
                    Content = n.Content
                })
                .ToListAsync();
        }
    }
}

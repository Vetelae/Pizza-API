using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities;
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

        public async Task<NewsDto?> GetNewsByIdAsync(int id)
        {
            var news = await _dbContext.News.FindAsync(id);

            if (news == null)
                return null;

            return new NewsDto
            {
                Id = news.Id,
                Date = news.Date,
                Title = news.Title,
                Content = news.Content
            };
        }

        public async Task<NewsDto> CreateNewsAsync(CreateNewsDto dto)
        {
            var news = new News
            {
                Date = dto.Date,
                Title = dto.Title,
                Content = dto.Content
            };

            _dbContext.News.Add(news);
            await _dbContext.SaveChangesAsync();

            return new NewsDto
            {
                Id = news.Id,
                Date = news.Date,
                Title = news.Title,
                Content = news.Content
            };
        }

        public async Task<NewsDto?> UpdateNewsAsync(int id, UpdateNewsDto dto)
        {
            var news = await _dbContext.News.FindAsync(id);
            if (news == null)
                return null;

            // Update the entity
            news.Date = dto.Date;
            news.Title = dto.Title;
            news.Content = dto.Content;

            // Save changes
            await _dbContext.SaveChangesAsync();

            // Return updated Dto
            return new NewsDto
            {
                Id = news.Id,
                Date = news.Date,
                Title = news.Title,
                Content = news.Content
            };
        }

        public async Task<bool> DeleteNewsAsync(int id)
        {
            var news = await _dbContext.News.FindAsync(id);
            if (news == null) 
                return false;

            _dbContext.News.Remove(news);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Pizza_API.Constants;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.News;
using Pizza_API.Exceptions;

namespace Pizza_API.Services
{
    public class NewsService : INewsService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IOutputCacheInvalidator _outputCacheInvalidator;

        public NewsService(
            ApplicationDbContext dbContext,
            IOutputCacheInvalidator outputCacheInvalidator)
        {
            _dbContext = dbContext;
            _outputCacheInvalidator = outputCacheInvalidator;
        }

        public async Task<List<NewsDto>> GetAllNewsAsync()
        {
            return await _dbContext.News
                .AsNoTracking()
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    Date = n.Date,
                    Title = n.Title,
                    Content = n.Content
                })
                .ToListAsync();
        }

        public async Task<NewsDto> GetNewsByIdAsync(int id)
        {
            var news = await _dbContext.News
                .AsNoTracking()
                .Where(n => n.Id == id)
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    Date = n.Date,
                    Title = n.Title,
                    Content = n.Content
                })
                .SingleOrDefaultAsync();

            if (news == null)
                throw new NotFoundException($"News {id} not found");

            return news;
        }

        public async Task<NewsDto> CreateNewsAsync(CreateNewsDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ValidationException("Title is required.");

            var news = new News
            {
                Date = dto.Date,
                Title = dto.Title,
                Content = dto.Content
            };

            _dbContext.News.Add(news);
            await _dbContext.SaveChangesAsync();
            await _outputCacheInvalidator.EvictByTagAsync(OutputCacheTags.News);

            return new NewsDto
            {
                Id = news.Id,
                Date = news.Date,
                Title = news.Title,
                Content = news.Content
            };
        }

        public async Task<NewsDto> UpdateNewsAsync(int id, UpdateNewsDto dto)
        {
            var news = await _dbContext.News.FindAsync(id);
            if (news == null)
                throw new NotFoundException($"News {id} not found");

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ValidationException("Title is required.");

            // Update the entity
            news.Date = dto.Date;
            news.Title = dto.Title;
            news.Content = dto.Content;

            // Save changes
            await _dbContext.SaveChangesAsync();
            await _outputCacheInvalidator.EvictByTagAsync(OutputCacheTags.News);

            // Return updated Dto
            return new NewsDto
            {
                Id = news.Id,
                Date = news.Date,
                Title = news.Title,
                Content = news.Content
            };
        }

        public async Task DeleteNewsAsync(int id)
        {
            var news = await _dbContext.News.FindAsync(id);
            if (news == null)
                throw new NotFoundException($"News {id} not found");

            _dbContext.News.Remove(news);
            await _dbContext.SaveChangesAsync();
            await _outputCacheInvalidator.EvictByTagAsync(OutputCacheTags.News);
        }
    }
}

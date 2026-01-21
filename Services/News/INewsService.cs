using Pizza_API.Entities.Dtos.News;

namespace Pizza_API.Services
{
    public interface INewsService
    {
        Task<List<NewsDto>> GetAllNewsAsync();
        Task<NewsDto?> GetNewsByIdAsync(int id);
        Task<NewsDto> CreateNewsAsync(CreateNewsDto dto);
        Task<NewsDto?> UpdateNewsAsync(int id, UpdateNewsDto dto);
        Task<bool> DeleteNewsAsync(int id);

    }
}

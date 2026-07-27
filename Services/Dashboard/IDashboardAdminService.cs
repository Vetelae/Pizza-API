using Pizza_API.Entities.Dtos.Dashboard;

namespace Pizza_API.Services
{
    public interface IDashboardAdminService
    {
        Task<TodayKpisDto> GetTodayKpisAsync(
            CancellationToken cancellationToken = default);
    }
}

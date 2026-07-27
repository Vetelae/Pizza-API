using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pizza_API.Data;
using Pizza_API.Entities.Dtos.Dashboard;
using Pizza_API.Enums;
using Pizza_API.Helpers;
using Pizza_API.Options;

namespace Pizza_API.Services
{
    public class DashboardAdminService : IDashboardAdminService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly TimeProvider _timeProvider;
        private readonly TimeZoneInfo _businessTimeZone;
        private readonly string _businessTimeZoneId;

        public DashboardAdminService(
            ApplicationDbContext dbContext,
            TimeProvider timeProvider,
            IOptions<BusinessOptions> businessOptions)
        {
            _dbContext = dbContext;
            _timeProvider = timeProvider;
            _businessTimeZoneId = businessOptions.Value.TimeZoneId;
            _businessTimeZone = TimeZoneHelper.FindSystemTimeZoneById(
                _businessTimeZoneId);
        }

        public async Task<TodayKpisDto> GetTodayKpisAsync(
            CancellationToken cancellationToken = default)
        {
            var generatedAt = _timeProvider.GetUtcNow();
            var businessDate = DateOnly.FromDateTime(
                TimeZoneInfo.ConvertTime(generatedAt, _businessTimeZone).DateTime);
            var startUtc = ToUtc(businessDate);
            var endUtc = ToUtc(businessDate.AddDays(1));

            var ordersToday = await _dbContext.Orders
                .AsNoTracking()
                .CountAsync(o =>
                    o.CreatedAt >= startUtc
                    && o.CreatedAt < endUtc
                    && o.Status != OrderStatus.Cancelled,
                    cancellationToken);

            var completedStats = await _dbContext.Orders
                .AsNoTracking()
                .Where(o =>
                    o.CompletedAt >= startUtc
                    && o.CompletedAt < endUtc)
                .GroupBy(_ => 1)
                .Select(group => new
                {
                    Count = group.Count(),
                    Revenue = group.Sum(o => o.TotalAmount)
                })
                .SingleOrDefaultAsync(cancellationToken);

            var preparationTimestamps = await _dbContext.Orders
                .AsNoTracking()
                .Where(o =>
                    o.PreparingAt.HasValue
                    && o.ReadyAt.HasValue
                    && o.ReadyAt >= startUtc
                    && o.ReadyAt < endUtc
                    && o.ReadyAt.Value >= o.PreparingAt.Value)
                .Select(o => new
                {
                    PreparingAt = o.PreparingAt!.Value,
                    ReadyAt = o.ReadyAt!.Value
                })
                .ToListAsync(cancellationToken);

            var activeCounts = await _dbContext.Orders
                .AsNoTracking()
                .Where(o =>
                    o.Status == OrderStatus.Pending
                    || o.Status == OrderStatus.Confirmed
                    || o.Status == OrderStatus.Preparing
                    || o.Status == OrderStatus.Ready)
                .GroupBy(_ => 1)
                .Select(group => new
                {
                    Pending = group.Count(o => o.Status == OrderStatus.Pending),
                    InProgress = group.Count(o =>
                        o.Status == OrderStatus.Confirmed
                        || o.Status == OrderStatus.Preparing
                        || o.Status == OrderStatus.Ready)
                })
                .SingleOrDefaultAsync(cancellationToken);

            var completedOrderCount = completedStats?.Count ?? 0;
            var revenueToday = completedStats?.Revenue ?? 0m;
            var averageOrderValue = completedOrderCount == 0
                ? 0m
                : decimal.Round(
                    revenueToday / completedOrderCount,
                    2,
                    MidpointRounding.AwayFromZero);
            var averagePrepTimeMinutes = preparationTimestamps.Count == 0
                ? (double?)null
                : Math.Round(
                    preparationTimestamps.Average(timestamp =>
                        (timestamp.ReadyAt - timestamp.PreparingAt).TotalMinutes),
                    1,
                    MidpointRounding.AwayFromZero);

            return new TodayKpisDto
            {
                Date = businessDate,
                TimeZone = _businessTimeZoneId,
                GeneratedAt = generatedAt,
                OrdersToday = ordersToday,
                RevenueToday = revenueToday,
                AverageOrderValue = averageOrderValue,
                AveragePrepTimeMinutes = averagePrepTimeMinutes,
                PendingOrders = activeCounts?.Pending ?? 0,
                OrdersInProgress = activeCounts?.InProgress ?? 0
            };
        }

        private DateTime ToUtc(DateOnly date)
        {
            var localDateTime = date.ToDateTime(
                TimeOnly.MinValue,
                DateTimeKind.Unspecified);

            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, _businessTimeZone);
        }
    }
}

namespace Pizza_API.Entities.Dtos.Dashboard
{
    public class TodayKpisDto
    {
        public DateOnly Date { get; set; }
        public string TimeZone { get; set; } = string.Empty;
        public DateTimeOffset GeneratedAt { get; set; }
        public int OrdersToday { get; set; }
        public decimal RevenueToday { get; set; }
        public decimal AverageOrderValue { get; set; }
        public double? AveragePrepTimeMinutes { get; set; }
        public int PendingOrders { get; set; }
        public int OrdersInProgress { get; set; }
    }
}

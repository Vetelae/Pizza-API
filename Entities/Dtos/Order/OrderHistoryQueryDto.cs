using Pizza_API.Constants;
using Pizza_API.Enums;

namespace Pizza_API.Entities.Dtos.Order
{
    public class OrderHistoryQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = OrderHistoryConstraints.DefaultPageSize;
        public OrderHistoryPeriod Period { get; set; } = OrderHistoryPeriod.Last30Days;
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public OrderStatus? Status { get; set; }
        public string? Search { get; set; }
    }
}

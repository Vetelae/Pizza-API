using Pizza_API.Enums;

namespace Pizza_API.Entities.Dtos.Order
{
    public class OrderHistoryItemDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public OrderType Type { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}

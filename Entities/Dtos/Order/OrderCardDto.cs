using Pizza_API.Enums;

namespace Pizza_API.Entities.Dtos.Order
{
    public class OrderCardDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StatusChangedAt { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}

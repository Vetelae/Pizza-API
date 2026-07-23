using Pizza_API.Enums;

namespace Pizza_API.Entities.Dtos.Order
{
    public class OrderStatusChangedDto
    {
        public int OrderId { get; set; }
        public OrderStatus OldStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }
        public OrderCardDto Order { get; set; } = new();
    }
}

using Pizza_API.Enums;

namespace Pizza_API.Entities.Dtos.Order
{
    public class CustomerOrderStatusChangedDto
    {
        public int OrderId { get; set; }
        public OrderStatus OldStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}

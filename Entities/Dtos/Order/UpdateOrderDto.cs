using Pizza_API.Entities.Dtos.OrderItem;

namespace Pizza_API.Entities.Dtos.Order
{
    public class UpdateOrderDto
    {
        // Customer info
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string DeliveryAddress { get; set; } = null!;

        // Order details
        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? Notes { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
using Pizza_API.Entities.Dtos.OrderItem;

namespace Pizza_API.Entities.Dtos.Order
{
    public class CreateOrderDto
    {
        // Only for authenticated users
        public string? UserId { get; set; }

        // Customer info
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string DeliveryAddress { get; set; } = null!;

        // Order details
        public OrderType Type { get; set; }
        public string PaymentMethod { get; set; } = null!; // "cash" or "card"
        public string? Notes { get; set; }

        // Items to order
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}

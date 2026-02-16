using Pizza_API.Entities.Dtos.OrderItem;

namespace Pizza_API.Entities.Dtos.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        // Customer info
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string DeliveryAddress { get; set; }

        // Order details
        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }  // IMPORTANT - add this!
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }  // IMPORTANT - add this!
        public string? Notes { get; set; }

        // For authenticated users 
        public string? UserId { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
using Pizza_API.Enums;

namespace Pizza_API.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional user reference (null for guest orders)
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Customer information (required for all orders)
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }

        // Delivery information
        public string DeliveryAddress { get; set; }
        public OrderType Type { get; set; } // Delivery or Pickup

        // Order status tracking
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Payment info (fake payment)
        public PaymentMethod PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }

        // Optional notes
        public string? Notes { get; set; }

        // Navigation property
        public List<OrderItem> Items { get; set; } = new();

        public string LookupToken { get; set; } = string.Empty;
    }
}
using System.ComponentModel.DataAnnotations;
using Pizza_API.Entities.Dtos.OrderItem;
using Pizza_API.Enums;

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
        [EnumDataType(typeof(OrderType), ErrorMessage = "Type must be either Pickup (0) or Delivery (1)")]
        public OrderType Type { get; set; }

        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }
        public string? Notes { get; set; }

        // Items to order
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}

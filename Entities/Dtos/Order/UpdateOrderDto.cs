using System.ComponentModel.DataAnnotations;
using Pizza_API.Entities.Dtos.OrderItem;
using Pizza_API.Enums;

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
        [EnumDataType(typeof(OrderType))]
        public OrderType Type { get; set; }

        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus Status { get; set; }

        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
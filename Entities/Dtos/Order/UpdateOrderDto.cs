using Pizza_API.Entities.Dtos.OrderItem;

namespace Pizza_API.Entities.Dtos.Order
{
    public class UpdateOrderDto
    {
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
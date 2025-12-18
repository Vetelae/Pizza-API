using Pizza_API.Entities.Dtos.OrderItem;

namespace Pizza_API.Entities.Dtos.Order
{
    public class CreateOrderDto
    {
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}

namespace Pizza_API.Entities.Dtos.OrderItem
{
    public class CreateOrderItemDto
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
    }
}
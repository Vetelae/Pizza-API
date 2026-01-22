namespace Pizza_API.Entities.Dtos.OrderItem
{
    public class OrderItemDto
    {
            public int MenuItemId { get; set; }
            public string MenuItemName { get; set; }
            public decimal MenuItemValue { get; set; }
            public int Quantity { get; set; }
    }
}
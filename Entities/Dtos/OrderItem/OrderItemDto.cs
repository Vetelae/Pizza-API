namespace Pizza_API.Entities.Dtos.OrderItem
{
    public class OrderItemDto
    {
            public int PizzaId { get; set; }
            public string PizzaName { get; set; }
            public decimal PizzaValue { get; set; }
            public int Quantity { get; set; }
    }
}
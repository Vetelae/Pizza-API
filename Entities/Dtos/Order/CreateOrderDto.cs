namespace Pizza_API.Entities.Dtos.Order
{
    public class CreateOrderDto
    {
        public int PizzaId { get; set; }
        public int Quantity { get; set; }
    }
}

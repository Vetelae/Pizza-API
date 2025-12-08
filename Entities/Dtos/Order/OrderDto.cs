namespace Pizza_API.Entities.Dtos.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string PizzaName { get; set; }
        public decimal PizzaValue { get; set; }
        public int Quantity { get; set; }
    }
}

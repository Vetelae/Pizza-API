namespace Pizza_API.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        // Quantity of this pizza
        public int Quantity { get; set; }

        // Foreign key to Pizza
        public int PizzaId { get; set; }
        public MenuItem Pizza { get; set; }

        // Foreign key to Order
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
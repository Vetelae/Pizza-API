namespace Pizza_API.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }

        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public string? Notes { get; set; } 
    }
}
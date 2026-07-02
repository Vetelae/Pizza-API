namespace Pizza_API.Entities.Dtos.CartItem
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }

        // Calculated field
        public decimal Total { get; set; }  // UnitPrice * Quantity
    }
}
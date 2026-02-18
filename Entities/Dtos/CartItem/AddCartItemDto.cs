namespace Pizza_API.Entities.Dtos.CartItem
{
    public class AddCartItemDto
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; } = 1;
        public string? Notes { get; set; }
    }
}
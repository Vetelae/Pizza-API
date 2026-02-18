using Pizza_API.Entities.Dtos.CartItem;

namespace Pizza_API.Entities.Dtos.Cart
{
    public class CartDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Cart items
        public List<CartItemDto> Items { get; set; } = new();

        // Calculated fields (not stored in DB, but useful for frontend)
        public int TotalItems { get; set; }  // Sum of all quantities
        public decimal Subtotal { get; set; }  // Sum of all item totals
    }
}
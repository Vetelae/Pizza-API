using System.ComponentModel.DataAnnotations;

namespace Pizza_API.Entities.Dtos.CartItem
{
    public class CartItemDto
    {
        public int Id { get; set; }

        [Required]
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = null!;
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(1, 99, ErrorMessage = "Quantity must be between 1 and 99")]
        public int Quantity { get; set; }

        [MaxLength(200)]
        public string? Notes { get; set; }

        // Calculated field
        public decimal Total { get; set; }  // UnitPrice * Quantity
    }
}
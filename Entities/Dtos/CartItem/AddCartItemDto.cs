using System.ComponentModel.DataAnnotations;
using Pizza_API.Constants;

namespace Pizza_API.Entities.Dtos.CartItem
{
    public class AddCartItemDto
    {
        public int MenuItemId { get; set; }

        [Range(CartItemConstraints.QuantityMin, CartItemConstraints.QuantityMax)]
        public int Quantity { get; set; } = 1;

        [StringLength(CartItemConstraints.NotesMaxLength)]
        public string? Notes { get; set; }
    }
}
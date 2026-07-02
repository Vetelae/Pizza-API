using System.ComponentModel.DataAnnotations;
using Pizza_API.Constants;

namespace Pizza_API.Entities.Dtos.CartItem
{
    public class UpdateCartItemDto
    {
        [Range(CartItemConstraints.QuantityMin, CartItemConstraints.QuantityMax)]
        public int Quantity { get; set; }

        [StringLength(CartItemConstraints.NotesMaxLength)]
        public string? Notes { get; set; }
    }
}
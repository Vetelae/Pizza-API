using System.ComponentModel.DataAnnotations;
using Pizza_API.Constants;

namespace Pizza_API.Entities.Dtos.OrderItem
{
    public class CreateOrderItemDto
    {
        public int MenuItemId { get; set; }

        [Range(CartItemConstraints.QuantityMin, CartItemConstraints.QuantityMax)]
        public int Quantity { get; set; }
    }
}
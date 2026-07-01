using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Cart;
using Pizza_API.Entities.Dtos.CartItem;

namespace Pizza_API.Helpers
{
    public class CartMappingHelper
    {
        // Helper method for cart mapping
        public static CartDto CartToDto(Cart cart)
        {
            var items = cart.CartItems.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                MenuItemId = ci.MenuItemId,
                MenuItemName = ci.MenuItem.Name,
                UnitPrice = ci.UnitPrice,
                Quantity = ci.Quantity,
                Notes = ci.Notes,
                Total = ci.UnitPrice * ci.Quantity
            }).ToList();

            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                SessionId = cart.SessionId,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                Items = items,
                TotalItems = items.Sum(i => i.Quantity),
                Subtotal = items.Sum(i => i.Total)
            };
        }
    }
}
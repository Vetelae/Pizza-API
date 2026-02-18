using Pizza_API.Entities.Dtos.Cart;
using Pizza_API.Entities.Dtos.CartItem;
using Pizza_API.Entities.Dtos.Order;

namespace Pizza_API.Services
{
    public interface ICartService
    {
        Task<CartDto?> GetCartAsync(string? userId, string? sessionId);
        Task<CartDto?> AddItemToCartAsync(string? userId, string? sessionId, AddCartItemDto dto);
        Task<CartDto?> UpdateCartItemAsync(string? userId, string? sessionId, int cartItemId, UpdateCartItemDto dto);
        Task<CartDto?> RemoveCartItemAsync(string? userId, string? sessionId, int cartItemId);
        Task<bool> ClearCartAsync(string? userId, string? sessionId);
        Task<OrderDto?> CheckoutAsync(string? userId, string? sessionId, CheckoutDto dto);
    }
}
using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Cart;
using Pizza_API.Entities.Dtos.CartItem;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Enums;
using Pizza_API.Exceptions;
using Pizza_API.Helpers;

namespace Pizza_API.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _dbContext;

        public CartService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GetCart
        public async Task<CartDto> GetCartAsync(string? userId, string? sessionId)
        {
            // Must have either userId or sessionId
            if (userId == null && sessionId == null)
                throw new ValidationException("UserId or SessionId is required.");

            // Find cart by userId or sessionId
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.MenuItem)
                .FirstOrDefaultAsync(c =>
                    (userId != null && c.UserId == userId) ||
                    (sessionId != null && c.SessionId == sessionId));

            return cart == null ? new CartDto() : CartMappingHelper.CartToDto(cart);
        }

        // AddItemToCart
        public async Task<CartDto> AddItemToCartAsync(string? userId, string? sessionId, AddCartItemDto dto)
        {
            // Must have either userId or sessionId
            if (userId == null && sessionId == null)
                throw new ValidationException("UserId or SessionId is required.");

            // Validate menu item exists and is available
            var menuItem = await _dbContext.MenuItems
                .FirstOrDefaultAsync(m => m.Id == dto.MenuItemId);

            if (menuItem == null)
                throw new NotFoundException($"MenuItem {dto.MenuItemId} not found.");

            if (!menuItem.IsAvailable)
                throw new ConflictException($"MenuItem {dto.MenuItemId} is not currently available.");

            // Find existing cart or create new one
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.MenuItem)
                .FirstOrDefaultAsync(c =>
                    (userId != null && c.UserId == userId) ||
                    (sessionId != null && c.SessionId == sessionId));

            // No cart exists yet - create one
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    SessionId = sessionId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _dbContext.Carts.Add(cart);
            }

            // Check if item already exists in cart
            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.MenuItemId == dto.MenuItemId);

            if (existingItem != null)
            {
                // Item already in cart - just increase quantity
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                // Add new item to cart
                cart.CartItems.Add(new CartItem
                {
                    MenuItemId = dto.MenuItemId,
                    Quantity = dto.Quantity,
                    UnitPrice = menuItem.Price, // Snapshot price
                    Notes = dto.Notes
                });
            }

            // Update cart timestamp
            cart.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return CartMappingHelper.CartToDto(cart);
        }

        // UpdateCartItem
        public async Task<CartDto> UpdateCartItemAsync(string? userId, string? sessionId, int cartItemId, UpdateCartItemDto dto)
        {
            // Must have either userId or sessionId
            if (userId == null && sessionId == null)
                throw new ValidationException("UserId or SessionId is required.");

            // Find cart with items
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.MenuItem)
                .FirstOrDefaultAsync(c =>
                    (userId != null && c.UserId == userId) ||
                    (sessionId != null && c.SessionId == sessionId));

            if (cart == null)
                throw new NotFoundException("Cart not found.");

            // Find the specific cart item
            // Security check: item must belong to THIS cart
            var cartItem = cart.CartItems
                .FirstOrDefault(ci => ci.Id == cartItemId);

            if (cartItem == null)
                throw new NotFoundException($"CartItem {cartItemId} not found.");

            // Update the item
            cartItem.Quantity = dto.Quantity;
            cartItem.Notes = dto.Notes;
            cart.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return CartMappingHelper.CartToDto(cart);
        }

        // RemoveCartItem
        public async Task<CartDto> RemoveCartItemAsync(string? userId, string? sessionId, int cartItemId)
        {
            // Must have either userId or sessionId
            if (userId == null && sessionId == null)
                throw new ValidationException("UserId or SessionId is required.");

            // Find cart with items
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.MenuItem)
                .FirstOrDefaultAsync(c =>
                    (userId != null && c.UserId == userId) ||
                    (sessionId != null && c.SessionId == sessionId));

            if (cart == null)
                throw new NotFoundException("Cart not found.");

            // Find the specific cart item
            // Security check: item must belong to THIS cart
            var cartItem = cart.CartItems
                .FirstOrDefault(ci => ci.Id == cartItemId);

            if (cartItem == null)
                throw new NotFoundException($"CartItem {cartItemId} not found.");

            // Check if it was the last item before removing
            var isLastItem = cart.CartItems.Count == 1;

            // Remove the item
            _dbContext.CartItems.Remove(cartItem);

            // If cart is empty, delete it
            if (isLastItem)
            {
                _dbContext.Carts.Remove(cart);
                await _dbContext.SaveChangesAsync();
                return new CartDto(); // Return empty cart
            }

            // Update timestamp for non-empty cart
            cart.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            // If cart still has items, return updated cart
            return CartMappingHelper.CartToDto(cart);
        }

        // ClearCart
        public async Task ClearCartAsync(string? userId, string? sessionId)
        {
            if (userId == null && sessionId == null)
                throw new ValidationException("UserId or SessionId is required.");

            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c =>
                    (userId != null && c.UserId == userId) ||
                    (sessionId != null && c.SessionId == sessionId));

            if (cart == null)
                return;

            // Delete the cart
            _dbContext.Carts.Remove(cart);
            await _dbContext.SaveChangesAsync();
        }

        // Checkout
        public async Task<OrderDto> CheckoutAsync(string? userId, string? sessionId, CheckoutDto dto)
        {
            // Must have either userId or sessionId
            if (userId == null && sessionId == null)
                throw new ValidationException("UserId or SessionId is required.");

            // Find cart with items
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.MenuItem)
                .FirstOrDefaultAsync(c =>
                    (userId != null && c.UserId == userId) ||
                    (sessionId != null && c.SessionId == sessionId));

            // No cart found or cart is empty
            if (cart == null)
                throw new NotFoundException("Cart not found.");

            if (!cart.CartItems.Any())
                throw new ValidationException("CartItem(s) are required.");

            // Validate all items are still available
            var unavailableItems = cart.CartItems
                .Where(ci => !ci.MenuItem.IsAvailable)
                .ToList();

            if (unavailableItems.Any())
                throw new ConflictException("Some CartItem(s) are not available.");

            // Calculate total
            var totalAmount = cart.CartItems
                .Sum(ci => ci.UnitPrice * ci.Quantity);

            // Create order from cart
            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,

                // Customer info from checkout form
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                CustomerPhone = dto.CustomerPhone,
                DeliveryAddress = dto.DeliveryAddress,

                // Order details
                Type = dto.Type,
                Status = OrderStatus.Pending,
                PaymentMethod = dto.PaymentMethod,
                TotalAmount = totalAmount,
                Notes = dto.Notes,

                LookupToken = Guid.NewGuid().ToString("N"),

                // Convert CartItems to OrderItems
                Items = cart.CartItems.Select(ci => new OrderItem
                {
                    MenuItemId = ci.MenuItemId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice // Already snapshotted when added to cart
                }).ToList()
            };

            // Save order
            _dbContext.Orders.Add(order);

            // Delete the cart after checkout
            _dbContext.CartItems.RemoveRange(cart.CartItems);
            _dbContext.Carts.Remove(cart);

            await _dbContext.SaveChangesAsync();

            // Load order with includes and return as OrderDto
            return await _dbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .Where(o => o.Id == order.Id)
                .Select(OrderMappingHelper.OrderToDto)
                .FirstOrDefaultAsync()
                ?? throw new InvalidOperationException($"Order {order.Id} was created but could not be reloaded.");
        }
    }
}
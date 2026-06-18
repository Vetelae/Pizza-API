using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Enums;
using Pizza_API.Exceptions;
using Pizza_API.Helpers;


namespace Pizza_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GetOrderById - for guest user
        public async Task<OrderDto> GetOrderByIdAsync(int id, string lookupToken)
        {
            var order = await _dbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .Where(o => o.Id == id && o.LookupToken == lookupToken)
                .Select(MappingHelper.OrderToDto)
                .FirstOrDefaultAsync();

            if (order == null)
                throw new NotFoundException($"Order {id} not found or lookup token is invalid");

            return order;
        }

        // CreateOrder
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
        {
            // Validate all MenuItems exist and are available
            var menuItemIds = dto.Items.Select(i => i.MenuItemId).Distinct().ToList();
            var menuItems = await _dbContext.MenuItems
                .Where(m => menuItemIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            // Check if all requested items exist
            var missingIds = menuItemIds.Except(menuItems.Keys).ToList();
            if (missingIds.Any())
                throw new NotFoundException($"Menu items not found: {string.Join(", ", missingIds)}");

            // Check if items are available
            var unavailableItems = menuItems.Values
                .Where(m => !m.IsAvailable)
                .Select(m => m.Name)
                .ToList();
            if (unavailableItems.Any())
                throw new ValidationException($"Items are currently unavailable: {string.Join(", ", unavailableItems)}");

            // Calculate total amount
            decimal totalAmount = dto.Items.Sum(i => menuItems[i.MenuItemId].Price * i.Quantity);

            // Create the order
            var order = new Order
            {
                CreatedAt = DateTime.UtcNow,

                // Customer info (from DTO)
                UserId = dto.UserId, // null for guest orders
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

                // Security token for guest order tracking
                LookupToken = Guid.NewGuid().ToString("N"),

                // Order items
                Items = dto.Items.Select(i => new OrderItem
                {
                    MenuItemId = i.MenuItemId,
                    Quantity = i.Quantity,
                    UnitPrice = menuItems[i.MenuItemId].Price // Snapshot the price
                }).ToList()
            };

            // Save to database
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            // Reload with includes for the DTO mapping
            var createdOrder = await _dbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .Where(o => o.Id == order.Id)
                .Select(MappingHelper.OrderToDto)
                .FirstOrDefaultAsync();

            if (createdOrder is null)
                throw new InvalidOperationException($"Order {order.Id} was created but could not be reloaded.");

            return createdOrder;
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Enums;
using Pizza_API.Exceptions;
using Pizza_API.Helpers;

namespace Pizza_API.Services
{
    public class OrderAdminService : IOrderAdminService
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderAdminService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GetAllOrders
        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            return await _dbContext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
                .Select(OrderMappingHelper.OrderToDto)
                .ToListAsync();
        }

        // GetOrdersByStatus
        public async Task<List<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _dbContext.Orders
                .Where(o => o.Status == status)
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .Select(OrderMappingHelper.OrderToDto)
                .ToListAsync();
        }

        // GetOrderByIdForAdmin
        public async Task<OrderDto> GetOrderByIdForAdminAsync(int id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .Where(o => o.Id == id)
                .Select(OrderMappingHelper.OrderToDto)
                .FirstOrDefaultAsync();

            if (order == null)
                throw new NotFoundException($"Order {id} not found");

            return order;
        }

        public async Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            // Load order with items
            var order = await _dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                throw new NotFoundException($"Order {id} not found");

            // Prevent updating completed/cancelled orders
            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
                throw new ConflictException($"Order {id} cannot be modified because it is already {order.Status}");

            // Update order-level fields
            order.CustomerName = dto.CustomerName;
            order.CustomerEmail = dto.CustomerEmail;
            order.CustomerPhone = dto.CustomerPhone;
            order.DeliveryAddress = dto.Type == OrderType.Delivery ? dto.DeliveryAddress : null;
            order.Type = dto.Type;
            order.Status = dto.Status;
            order.PaymentMethod = dto.PaymentMethod;
            order.Notes = dto.Notes;

            // Validate MenuItems exist and are available
            var menuItemIds = dto.Items.Select(i => i.MenuItemId).Distinct().ToList();
            var menuItems = await _dbContext.MenuItems
                .Where(m => menuItemIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            var missingIds = menuItemIds.Except(menuItems.Keys).ToList();
            if (missingIds.Any())
                throw new NotFoundException($"Menu items not found: {string.Join(", ", missingIds)}");

            // Optional: Check availability
            var unavailableItems = menuItems.Values
                .Where(m => !m.IsAvailable)
                .Select(m => m.Name)
                .ToList();
            if (unavailableItems.Any())
                throw new ValidationException($"Items are currently unavailable: {string.Join(", ", unavailableItems)}");

            // --- UPDATE & ADD ITEMS ---
            foreach (var incomingItem in dto.Items)
            {
                var existingItem = order.Items
                    .FirstOrDefault(i => i.MenuItemId == incomingItem.MenuItemId);

                if (existingItem != null)
                {
                    // Update quantity
                    existingItem.Quantity = incomingItem.Quantity;
                }
                else
                {
                    // Add new MenuItem to order
                    order.Items.Add(new OrderItem
                    {
                        MenuItemId = incomingItem.MenuItemId,
                        Quantity = incomingItem.Quantity,
                        UnitPrice = menuItems[incomingItem.MenuItemId].Price
                    });
                }
            }

            // --- REMOVE DELETED ITEMS ---
            var incomingMenuItemIds = dto.Items.Select(i => i.MenuItemId).ToHashSet();
            var itemsToRemove = order.Items
                .Where(i => !incomingMenuItemIds.Contains(i.MenuItemId))
                .ToList();

            _dbContext.OrderItems.RemoveRange(itemsToRemove);

            // Recalculate total amount
            order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

            // Save changes
            await _dbContext.SaveChangesAsync();

            // Reload and map using helper method
            var updatedOrder = await _dbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .Where(o => o.Id == order.Id)
                .Select(OrderMappingHelper.OrderToDto)
                .FirstOrDefaultAsync();

            if (updatedOrder is null)
                throw new InvalidOperationException($"Order {order.Id} was updated but could not be reloaded.");

            return updatedOrder;
        }

        // DeleteOrder
        public async Task DeleteOrderAsync(int id)
        {
            var order = await _dbContext.Orders.FindAsync(id);

            if (order == null)
                throw new NotFoundException($"Order {id} not found");

            // Prevent deleting completed orders
            if (order.Status == OrderStatus.Completed)
                throw new ConflictException($"Order {id} cannot be deleted because it is already {order.Status}");

            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
        }
    }
}
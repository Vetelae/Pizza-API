using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Entities.Dtos.OrderItem;
using Pizza_API.Enums;
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


        // GetAllOrders
        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            return await _dbContext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
                .Select(MappingHelper.OrderToDto)
                .ToListAsync();
        }

        // GetOrdersByStatus
        public async Task<List<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _dbContext.Orders
                .Where(o => o.Status == status)
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .Select(MappingHelper.OrderToDto)
                .ToListAsync();
        }

        // GetOrdersByUser
        public async Task<List<OrderDto>> GetOrdersByUserAsync(string userId)
        {
            return await _dbContext.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .OrderByDescending(o => o.CreatedAt)
                .Select(MappingHelper.OrderToDto)
                .ToListAsync();
        }

        // GetOrderById
        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            return await _dbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .Where(o => o.Id == id)
                .Select(MappingHelper.OrderToDto)
                .FirstOrDefaultAsync();
        }

        // CreateOrder
        public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto)
        {
            // Validate all MenuItems exist and are available
            var menuItemIds = dto.Items.Select(i => i.MenuItemId).Distinct().ToList();
            var menuItems = await _dbContext.MenuItems
                .Where(m => menuItemIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            // Check if all requested items exist
            if (menuItems.Count != menuItemIds.Count)
                return null; // Some menu items don't exist

            // Check if items are available
            var unavailableItems = menuItems.Values.Where(m => !m.IsAvailable).ToList();
            if (unavailableItems.Any())
                return null; // Or throw exception with details about unavailable items

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

            return createdOrder;
        }

        public async Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            // Load order with items
            var order = await _dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return null;

            // Prevent updating completed/cancelled orders
            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
                return null;

            // Update order-level fields
            order.CustomerName = dto.CustomerName;
            order.CustomerEmail = dto.CustomerEmail;
            order.CustomerPhone = dto.CustomerPhone;
            order.DeliveryAddress = dto.DeliveryAddress;
            order.Type = dto.Type;
            order.Status = dto.Status;
            order.PaymentMethod = dto.PaymentMethod;
            order.Notes = dto.Notes;

            // Validate MenuItems exist and are available
            var menuItemIds = dto.Items.Select(i => i.MenuItemId).Distinct().ToList();
            var menuItems = await _dbContext.MenuItems
                .Where(m => menuItemIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            if (menuItems.Count != menuItemIds.Count)
                return null;

            // Optional: Check availability
            var unavailableItems = menuItems.Values.Where(m => !m.IsAvailable).ToList();
            if (unavailableItems.Any())
                return null;

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
                .Select(MappingHelper.OrderToDto)
                .FirstOrDefaultAsync();

            return updatedOrder;
        }

        // DeleteOrder
        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _dbContext.Orders.FindAsync(id);

            if (order == null)
                return false;

            // Prevent deleting completed orders
            if (order.Status == OrderStatus.Completed)
                return false;

            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
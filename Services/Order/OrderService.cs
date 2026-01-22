using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Entities.Dtos.OrderItem;


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
                .Select(order => new OrderDto
                {
                    Id = order.Id,
                    CreatedAt = order.CreatedAt,
                    Items = order.Items.Select(i => new OrderItemDto
                    {
                        MenuItemId = i.MenuItemId,
                        MenuItemName = i.MenuItem.Name,
                        MenuItemValue = i.UnitPrice,
                        Quantity = i.Quantity
                    }).ToList()
                })
            .ToListAsync();
        }

        // GetOrderById
        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return null;

            return new OrderDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    MenuItemId = i.Id,
                    MenuItemName = i.MenuItem.Name,
                    MenuItemValue = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        // CreateOrder
        public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto)
        {
            // Check all MenuItems exist
            foreach (var item in dto.Items)
            {
                var menuItemExists = await _dbContext.MenuItems.AnyAsync(m => m.Id == item.MenuItemId);
                if (!menuItemExists)
                    return null;
            }

            var menuItems = await _dbContext.MenuItems
                .Where(m => dto.Items.Select(i => i.MenuItemId).Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            // Create the order
            var order = new Order
            {
                CreatedAt = DateTime.UtcNow,
                Items = dto.Items.Select(i => new OrderItem
                {
                MenuItemId = i.MenuItemId,
                Quantity = i.Quantity,
                UnitPrice = menuItems[i.MenuItemId].Price
                }).ToList()
            };

            // Save to database
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            // Map to DTO
            return new OrderDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    MenuItemId = i.MenuItemId,
                    MenuItemName = menuItems[i.MenuItemId].Name,
                    MenuItemValue = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        public async Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            // Load order with items
            var order = await _dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return null;

            // Validate MenuItems exist
            var menuItemIds = dto.Items.Select(i => i.MenuItemId).Distinct().ToList();
            var menuItems = await _dbContext.MenuItems
                .Where(m => menuItemIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            if (menuItems.Count != menuItemIds.Count)
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

            // Save changes
            await _dbContext.SaveChangesAsync();

            // Map to DTO
            return new OrderDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    MenuItemId = i.MenuItemId,
                    MenuItemName = menuItems[i.MenuItemId].Name,
                    MenuItemValue = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        // DeleteOrder
        public async Task <bool> DeleteOrderAsync(int id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return false;

            _dbContext.OrderItems.RemoveRange(order.Items);
            _dbContext.Orders.Remove(order);

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
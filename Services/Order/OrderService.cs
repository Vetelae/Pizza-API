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
                .ThenInclude(i => i.Pizza)
                .Select(order => new OrderDto
                {
                    Id = order.Id,
                    CreatedAt = order.CreatedAt,
                    Items = order.Items.Select(i => new OrderItemDto
                    {
                        PizzaId = i.PizzaId,
                        PizzaName = i.Pizza.Name,
                        PizzaValue = i.Pizza.Value,
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
                .ThenInclude(i => i.Pizza)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return null;

            return new OrderDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    PizzaId = i.PizzaId,
                    PizzaName = i.Pizza.Name,
                    PizzaValue = i.Pizza.Value,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        // CreateOrder
        public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto)
        {
            // Check all pizzas exist
            foreach (var item in dto.Items)
            {
                var pizzaExists = await _dbContext.Pizzas.AnyAsync(p => p.Id == item.PizzaId);
                if (!pizzaExists)
                    return null;
            }

            var pizzas = await _dbContext.Pizzas
                .Where(p => dto.Items.Select(i => i.PizzaId).Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            // Create the order
            var order = new Order
            {
                CreatedAt = DateTime.UtcNow,
                Items = dto.Items.Select(i => new OrderItem
                {
                PizzaId = i.PizzaId,
                Quantity = i.Quantity
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
                Items = dto.Items.Select(i => new OrderItemDto
                {
                    PizzaId = i.PizzaId,
                    PizzaName = pizzas[i.PizzaId].Name,
                    PizzaValue = pizzas[i.PizzaId].Value,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        public async Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            // Load order with items
            var order = await _dbContext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Pizza)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return null;

            // Validate pizzas exist
            var pizzaIds = dto.Items.Select(i => i.PizzaId).Distinct().ToList();
            var pizzas = await _dbContext.Pizzas
                .Where(p => pizzaIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            if (pizzas.Count != pizzaIds.Count)
                return null;

            // --- UPDATE & ADD ITEMS ---
            foreach (var incomingItem in dto.Items)
            {
                var existingItem = order.Items
                    .FirstOrDefault(i => i.PizzaId == incomingItem.PizzaId);

                if (existingItem != null)
                {
                    // Update quantity
                    existingItem.Quantity = incomingItem.Quantity;
                }
                else
                {
                    // Add new pizza to order
                    order.Items.Add(new OrderItem
                    {
                        PizzaId = incomingItem.PizzaId,
                        Quantity = incomingItem.Quantity
                    });
                }
            }

            // --- REMOVE DELETED ITEMS ---
            var incomingPizzaIds = dto.Items.Select(i => i.PizzaId).ToHashSet();

            var itemsToRemove = order.Items
                .Where(i => !incomingPizzaIds.Contains(i.PizzaId))
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
                    PizzaId = i.PizzaId,
                    PizzaName = pizzas[i.PizzaId].Name,
                    PizzaValue = pizzas[i.PizzaId].Value,
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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        public List<OrderDto> GetAllOrders()
        {
            return _dbContext.Orders
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
            .ToList();
        }

        // GetOrderById
        public OrderDto? GetOrderById(int id)
        {
            var order = _dbContext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Pizza)
                .FirstOrDefault(o => o.Id == id);

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
        public OrderDto? CreateOrder(CreateOrderDto dto)
        {
            // Check all pizzas exist
            foreach (var item in dto.Items)
            {
                var pizzaExists = _dbContext.Pizzas.Any(p => p.Id == item.PizzaId);
                if (!pizzaExists)
                    return null;
            }

            var pizzas = _dbContext.Pizzas
                .Where(p => dto.Items.Select(i => i.PizzaId).Contains(p.Id))
                .ToDictionary(p => p.Id);

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
            _dbContext.SaveChanges();

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

        public OrderDto? UpdateOrder(int id, UpdateOrderDto dto)
        {
            // Load order with items
            var order = _dbContext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Pizza)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return null;

            // Validate pizzas exist
            var pizzaIds = dto.Items.Select(i => i.PizzaId).Distinct().ToList();
            var pizzas = _dbContext.Pizzas
                .Where(p => pizzaIds.Contains(p.Id))
                .ToDictionary(p => p.Id);

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
            _dbContext.SaveChanges();

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


        // UpdateOrder
        //public OrderDto? UpdateOrder(int id, UpdateOrderDto dto)
        //{
        //    var order = _dbContext.Orders.Find(id);
        //    if (order == null)
        //        return null;

        //    var pizza = _dbContext.Pizzas.Find(dto.PizzaId);
        //    if (pizza == null)
        //        return null;

        //    // Update the entity
        //    order.PizzaId = dto.PizzaId;
        //    order.Quantity = dto.Quantity;

        //    // Save changes
        //    _dbContext.SaveChanges();

        //    // Return updated DTO
        //    return new OrderDto
        //    {
        //        Id = order.Id,
        //        PizzaName = pizza.Name,
        //        PizzaValue = pizza.Value,
        //        Quantity = order.Quantity
        //    };
        //}

        // DeleteOrder
        public bool DeleteOrder(int id)
        {
            var order = _dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return false;

            _dbContext.OrderItems.RemoveRange(order.Items);
            _dbContext.Orders.Remove(order);

            _dbContext.SaveChanges();
            return true;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Order;


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
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    PizzaName = o.Pizza.Name,
                    PizzaValue = o.Pizza.Value,
                    Quantity = o.Quantity
            })
            .ToList();
        }

        // GetOrderById
        public OrderDto? GetOrderById(int id)
        {
            var order = _dbContext.Orders
                .Include(o => o.Pizza)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return null;

            return new OrderDto
            {
                Id = order.Id,
                PizzaName = order.Pizza.Name,
                PizzaValue = order.Pizza.Value,
                Quantity = order.Quantity
            };
        }

        // CreateOrder
        public OrderDto? CreateOrder(CreateOrderDto dto)
        {
            // Check if pizza exists
            var pizza = _dbContext.Pizzas.Find(dto.PizzaId);
            if (pizza == null)
                return null;

            // Create Order entity
            var order = new Order
            {
                PizzaId = dto.PizzaId,
                Quantity = dto.Quantity
            };

            // Save to database
            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            // Map to DTO
            return new OrderDto
            {
                Id = order.Id,
                PizzaName = pizza.Name,
                PizzaValue= pizza.Value,
                Quantity= dto.Quantity
            };
        }

        // UpdateOrder
        public OrderDto? UpdateOrder(int id, UpdateOrderDto dto)
        {
            var order = _dbContext.Orders.Find(id);
            if (order == null)
                return null;

            var pizza = _dbContext.Pizzas.Find(dto.PizzaId);
            if (pizza == null)
                return null;

            // Update the entity
            order.PizzaId = dto.PizzaId;
            order.Quantity = dto.Quantity;

            // Save changes
            _dbContext.SaveChanges();

            // Return updated DTO
            return new OrderDto
            {
                Id = order.Id,
                PizzaName = pizza.Name,
                PizzaValue = pizza.Value,
                Quantity= order.Quantity
            };
        }

        // DeleteOrder
        public bool DeleteOrder(int id)
        {
            var order = _dbContext.Orders.Find(id);
            if (order == null) 
                return false;

            _dbContext.Orders.Remove(order);
            _dbContext.SaveChanges();
            return true;
        }
    }
}

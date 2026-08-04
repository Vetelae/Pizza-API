using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities.Dtos.Order;
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
                .Select(OrderMappingHelper.OrderToDto)
                .FirstOrDefaultAsync();

            if (order == null)
                throw new NotFoundException($"Order {id} not found or lookup token is invalid");

            return order;
        }
    }
}

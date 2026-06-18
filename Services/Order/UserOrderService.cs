using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Exceptions;
using Pizza_API.Helpers;

namespace Pizza_API.Services
{
    public class UserOrderService : IUserOrderService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserOrderService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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

        // GetOrderByIdForUser - for authenticated user
        public async Task<OrderDto> GetOrderByIdForUserAsync(int id, string userId)
        {
            var order = await _dbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .Where(o => o.Id == id && o.UserId == userId)
                .Select(MappingHelper.OrderToDto)
                .FirstOrDefaultAsync();

            if (order == null)
                throw new NotFoundException($"Order {id} not found");

            return order;
        }
    }
}
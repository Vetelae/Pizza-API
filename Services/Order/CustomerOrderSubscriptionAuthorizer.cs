using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;

namespace Pizza_API.Services
{
    public class CustomerOrderSubscriptionAuthorizer : ICustomerOrderSubscriptionAuthorizer
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerOrderSubscriptionAuthorizer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> CanSubscribeAsync(int orderId, string lookupToken)
        {
            return _dbContext.Orders
                .AsNoTracking()
                .AnyAsync(order =>
                    order.Id == orderId &&
                    order.LookupToken == lookupToken);
        }
    }
}

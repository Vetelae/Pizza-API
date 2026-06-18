using Pizza_API.Entities.Dtos.Order;

namespace Pizza_API.Services
{
    public interface IUserOrderService
    {
        Task<List<OrderDto>> GetOrdersByUserAsync(string userId);
        Task<OrderDto> GetOrderByIdForUserAsync(int id, string userId);
    }
}
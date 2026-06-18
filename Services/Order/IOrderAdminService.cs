using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Enums;

namespace Pizza_API.Services
{
    public interface IOrderAdminService
    {
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<List<OrderDto>> GetOrdersByStatusAsync(OrderStatus status);
        Task<OrderDto> GetOrderByIdForAdminAsync(int id);
        Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto dto);
        Task DeleteOrderAsync(int id);
    }
}
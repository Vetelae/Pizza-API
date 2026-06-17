using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Enums;


namespace Pizza_API.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<List<OrderDto>> GetOrdersByStatusAsync(OrderStatus status);
        Task<List<OrderDto>> GetOrdersByUserAsync(string userId);
        Task<OrderDto> GetOrderByIdAsync(int id, string lookupToken);
        Task<OrderDto> GetOrderByIdForUserAsync(int id, string userId);
        Task<OrderDto> GetOrderByIdForAdminAsync(int id);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
        Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto dto);
        Task DeleteOrderAsync(int id);
    }
}
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Entities.Dtos.OrderItem;
using Pizza_API.Enums;


namespace Pizza_API.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<List<OrderDto>> GetOrdersByStatusAsync(OrderStatus status);
        Task<List<OrderDto>> GetOrdersByUserAsync(string userId);
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<OrderDto?> CreateOrderAsync(CreateOrderDto dto);
        Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto);
        Task<bool> DeleteOrderAsync(int id);
    }
}
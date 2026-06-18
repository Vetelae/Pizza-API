using Pizza_API.Entities.Dtos.Order;

namespace Pizza_API.Services
{
    public interface IOrderService
    {
        Task<OrderDto> GetOrderByIdAsync(int id, string lookupToken);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
    }
}
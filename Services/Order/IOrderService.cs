using Pizza_API.Entities.Dtos.Order;

namespace Pizza_API.Services
{
    public interface IOrderService
    {
        List<OrderDto> GetAllOrders();
        OrderDto? GetOrderById(int id);
        OrderDto? CreateOrder(CreateOrderDto dto);
        OrderDto? UpdateOrder(int id, UpdateOrderDto dto);
        bool DeleteOrder(int id);
    }
}

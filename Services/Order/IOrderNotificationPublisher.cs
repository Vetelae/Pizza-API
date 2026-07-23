using Pizza_API.Entities.Dtos.Order;

namespace Pizza_API.Services
{
    public interface IOrderNotificationPublisher
    {
        Task OrderCreatedAsync(OrderCardDto order);
        Task OrderStatusChangedAsync(OrderStatusChangedDto change);
        Task OrderUpdatedAsync(OrderCardDto order);
    }
}

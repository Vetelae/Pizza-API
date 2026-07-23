using Pizza_API.Entities.Dtos.Order;

namespace Pizza_API.Hubs
{
    public interface IOrderNotificationClient
    {
        Task OrderCreated(OrderCardDto order);
        Task OrderStatusChanged(OrderStatusChangedDto change);
        Task OrderUpdated(OrderCardDto order);
    }
}

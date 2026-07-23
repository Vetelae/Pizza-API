using Pizza_API.Entities.Dtos.Order;

namespace Pizza_API.Hubs
{
    public interface ICustomerOrderNotificationClient
    {
        Task OrderStatusChanged(CustomerOrderStatusChangedDto change);
    }
}

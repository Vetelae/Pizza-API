using Microsoft.AspNetCore.SignalR;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Hubs;

namespace Pizza_API.Services
{
    public class OrderNotificationPublisher : IOrderNotificationPublisher
    {
        private readonly IHubContext<OrderNotificationHub, IOrderNotificationClient> _hubContext;
        private readonly ILogger<OrderNotificationPublisher> _logger;

        public OrderNotificationPublisher(
            IHubContext<OrderNotificationHub, IOrderNotificationClient> hubContext,
            ILogger<OrderNotificationPublisher> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public Task OrderCreatedAsync(OrderCardDto order)
        {
            return PublishAsync(
                client => client.OrderCreated(order),
                "OrderCreated",
                order.Id);
        }

        public Task OrderStatusChangedAsync(OrderStatusChangedDto change)
        {
            return PublishAsync(
                client => client.OrderStatusChanged(change),
                "OrderStatusChanged",
                change.OrderId);
        }

        public Task OrderUpdatedAsync(OrderCardDto order)
        {
            return PublishAsync(
                client => client.OrderUpdated(order),
                "OrderUpdated",
                order.Id);
        }

        private async Task PublishAsync(
            Func<IOrderNotificationClient, Task> publish,
            string eventName,
            int orderId)
        {
            try
            {
                await publish(_hubContext.Clients.Group(OrderNotificationHub.AdminOrdersGroup));
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to publish {EventName} for order {OrderId}",
                    eventName,
                    orderId);
            }
        }
    }
}

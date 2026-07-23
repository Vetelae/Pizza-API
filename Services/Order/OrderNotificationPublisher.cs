using Microsoft.AspNetCore.SignalR;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Hubs;

namespace Pizza_API.Services
{
    public class OrderNotificationPublisher : IOrderNotificationPublisher
    {
        private readonly IHubContext<OrderNotificationHub, IOrderNotificationClient> _adminHubContext;
        private readonly IHubContext<CustomerOrderNotificationHub, ICustomerOrderNotificationClient> _customerHubContext;
        private readonly ILogger<OrderNotificationPublisher> _logger;

        public OrderNotificationPublisher(
            IHubContext<OrderNotificationHub, IOrderNotificationClient> adminHubContext,
            IHubContext<CustomerOrderNotificationHub, ICustomerOrderNotificationClient> customerHubContext,
            ILogger<OrderNotificationPublisher> logger)
        {
            _adminHubContext = adminHubContext;
            _customerHubContext = customerHubContext;
            _logger = logger;
        }

        public Task OrderCreatedAsync(OrderCardDto order)
        {
            return PublishToAdminAsync(
                client => client.OrderCreated(order),
                "OrderCreated",
                order.Id);
        }

        public async Task OrderStatusChangedAsync(
            OrderStatusChangedDto change,
            string? userId)
        {
            var customerChange = new CustomerOrderStatusChangedDto
            {
                OrderId = change.OrderId,
                OldStatus = change.OldStatus,
                NewStatus = change.NewStatus,
                ChangedAt = change.ChangedAt
            };

            var customerGroups = new List<string>
            {
                CustomerOrderNotificationHub.GetOrderGroupName(change.OrderId)
            };

            if (!string.IsNullOrWhiteSpace(userId))
            {
                customerGroups.Add(
                    CustomerOrderNotificationHub.GetUserGroupName(userId));
            }

            await Task.WhenAll(
                PublishToAdminAsync(
                    client => client.OrderStatusChanged(change),
                    "OrderStatusChanged",
                    change.OrderId),
                PublishToCustomerAsync(
                    customerGroups,
                    client => client.OrderStatusChanged(customerChange),
                    "OrderStatusChanged",
                    change.OrderId));
        }

        public Task OrderUpdatedAsync(OrderCardDto order)
        {
            return PublishToAdminAsync(
                client => client.OrderUpdated(order),
                "OrderUpdated",
                order.Id);
        }

        private async Task PublishToAdminAsync(
            Func<IOrderNotificationClient, Task> publish,
            string eventName,
            int orderId)
        {
            try
            {
                await publish(
                    _adminHubContext.Clients.Group(
                        OrderNotificationHub.AdminOrdersGroup));
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to publish {EventName} to admins for order {OrderId}",
                    eventName,
                    orderId);
            }
        }

        private async Task PublishToCustomerAsync(
            IReadOnlyList<string> groupNames,
            Func<ICustomerOrderNotificationClient, Task> publish,
            string eventName,
            int orderId)
        {
            try
            {
                await publish(_customerHubContext.Clients.Groups(groupNames));
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to publish {EventName} to customers for order {OrderId}",
                    eventName,
                    orderId);
            }
        }
    }
}

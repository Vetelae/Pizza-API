using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Pizza_API.Services;

namespace Pizza_API.Hubs
{
    [AllowAnonymous]
    public sealed class CustomerOrderNotificationHub : Hub<ICustomerOrderNotificationClient>
    {
        private readonly ICustomerOrderSubscriptionAuthorizer _subscriptionAuthorizer;

        public CustomerOrderNotificationHub(
            ICustomerOrderSubscriptionAuthorizer subscriptionAuthorizer)
        {
            _subscriptionAuthorizer = subscriptionAuthorizer;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    GetUserGroupName(userId));
            }

            await base.OnConnectedAsync();
        }

        public async Task SubscribeToOrder(int orderId, string lookupToken)
        {
            if (orderId <= 0 ||
                string.IsNullOrWhiteSpace(lookupToken) ||
                !await _subscriptionAuthorizer.CanSubscribeAsync(orderId, lookupToken))
            {
                throw new HubException("Order was not found or lookup token is invalid.");
            }

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                GetOrderGroupName(orderId));
        }

        public static string GetOrderGroupName(int orderId)
        {
            return $"customer-order:{orderId}";
        }

        public static string GetUserGroupName(string userId)
        {
            return $"customer-user:{userId}";
        }
    }
}

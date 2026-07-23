using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Pizza_API.Hubs
{
    [Authorize(Roles = "Admin")]
    public sealed class OrderNotificationHub : Hub<IOrderNotificationClient>
    {
        public const string AdminOrdersGroup = "admin-orders";

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, AdminOrdersGroup);
            await base.OnConnectedAsync();
        }
    }
}

using System.Linq.Expressions;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Entities.Dtos.OrderItem;

namespace Pizza_API.Helpers
{
    public class OrderMappingHelper
    {
        // Helper method for order mapping
        public static Expression<Func<Order, OrderDto>> OrderToDto => order => new OrderDto
        {
            Id = order.Id,
            CreatedAt = order.CreatedAt,
            // Customer info
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            CustomerPhone = order.CustomerPhone,
            DeliveryAddress = order.DeliveryAddress,
            // Order details
            Type = order.Type,
            Status = order.Status,
            PaymentMethod = order.PaymentMethod,
            TotalAmount = order.TotalAmount,
            Notes = order.Notes,
            UserId = order.UserId,
            LookupToken = order.LookupToken,
            // Items
            Items = order.Items.Select(i => new OrderItemDto
            {
                MenuItemId = i.MenuItemId,
                MenuItemName = i.MenuItem.Name,
                MenuItemValue = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}
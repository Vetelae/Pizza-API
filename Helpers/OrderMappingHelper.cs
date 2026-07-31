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
            StatusChangedAt = order.StatusChangedAt,
            ConfirmedAt = order.ConfirmedAt,
            PreparingAt = order.PreparingAt,
            ReadyAt = order.ReadyAt,
            CompletedAt = order.CompletedAt,
            CancelledAt = order.CancelledAt,
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

        public static Expression<Func<Order, OrderCardDto>> OrderToCardDto => order => new OrderCardDto
        {
            Id = order.Id,
            CreatedAt = order.CreatedAt,
            StatusChangedAt = order.StatusChangedAt,
            CustomerName = order.CustomerName,
            Type = order.Type,
            Status = order.Status,
            ItemCount = order.Items.Sum(i => i.Quantity),
            TotalAmount = order.TotalAmount
        };

        public static Expression<Func<Order, OrderHistoryItemDto>> OrderToHistoryItemDto => order => new OrderHistoryItemDto
        {
            Id = order.Id,
            CreatedAt = order.CreatedAt,
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            Type = order.Type,
            Status = order.Status,
            ItemCount = order.Items.Sum(i => i.Quantity),
            TotalAmount = order.TotalAmount
        };

        public static OrderCardDto ToOrderCardDto(OrderDto order)
        {
            return new OrderCardDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                StatusChangedAt = order.StatusChangedAt,
                CustomerName = order.CustomerName,
                Type = order.Type,
                Status = order.Status,
                ItemCount = order.Items.Sum(i => i.Quantity),
                TotalAmount = order.TotalAmount
            };
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Enums;
using Pizza_API.Exceptions;
using Pizza_API.Helpers;

namespace Pizza_API.Services
{
    public class OrderAdminService : IOrderAdminService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IOrderNotificationPublisher _notificationPublisher;

        public OrderAdminService(
            ApplicationDbContext dbContext,
            IOrderNotificationPublisher notificationPublisher)
        {
            _dbContext = dbContext;
            _notificationPublisher = notificationPublisher;
        }

        // GetAllOrders
        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            return await _dbContext.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
                .Select(OrderMappingHelper.OrderToDto)
                .ToListAsync();
        }

        public async Task<List<OrderCardDto>> GetActiveOrdersAsync()
        {
            var activeStatuses = new[]
            {
                OrderStatus.Pending,
                OrderStatus.Confirmed,
                OrderStatus.Preparing,
                OrderStatus.Ready
            };

            return await _dbContext.Orders
                .AsNoTracking()
                .Where(o => activeStatuses.Contains(o.Status))
                .OrderBy(o => o.CreatedAt)
                .Select(OrderMappingHelper.OrderToCardDto)
                .ToListAsync();
        }

        // GetOrdersByStatus
        public async Task<List<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _dbContext.Orders
                .AsNoTracking()
                .Where(o => o.Status == status)
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .Select(OrderMappingHelper.OrderToDto)
                .ToListAsync();
        }

        // GetOrderByIdForAdmin
        public async Task<OrderDto> GetOrderByIdForAdminAsync(int id)
        {
            var order = await _dbContext.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .Where(o => o.Id == id)
                .Select(OrderMappingHelper.OrderToDto)
                .FirstOrDefaultAsync();

            if (order == null)
                throw new NotFoundException($"Order {id} not found");

            return order;
        }

        public async Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            // Load order with items
            var order = await _dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                throw new NotFoundException($"Order {id} not found");

            // Prevent updating completed/cancelled orders
            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
                throw new ConflictException($"Order {id} cannot be modified because it is already {order.Status}");

            var oldStatus = order.Status;
            var statusChanged = ApplyStatusTransition(order, dto.Status);

            // Update order-level fields
            order.CustomerName = dto.CustomerName;
            order.CustomerEmail = dto.CustomerEmail;
            order.CustomerPhone = dto.CustomerPhone;
            order.DeliveryAddress = dto.Type == OrderType.Delivery ? dto.DeliveryAddress : null;
            order.Type = dto.Type;
            order.PaymentMethod = dto.PaymentMethod;
            order.Notes = dto.Notes;

            // Validate MenuItems exist and are available
            var menuItemIds = dto.Items.Select(i => i.MenuItemId).Distinct().ToList();
            var menuItems = await _dbContext.MenuItems
                .Where(m => menuItemIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            var missingIds = menuItemIds.Except(menuItems.Keys).ToList();
            if (missingIds.Any())
                throw new NotFoundException($"Menu items not found: {string.Join(", ", missingIds)}");

            // Optional: Check availability
            var unavailableItems = menuItems.Values
                .Where(m => !m.IsAvailable)
                .Select(m => m.Name)
                .ToList();
            if (unavailableItems.Any())
                throw new ValidationException($"Items are currently unavailable: {string.Join(", ", unavailableItems)}");

            // --- UPDATE & ADD ITEMS ---
            foreach (var incomingItem in dto.Items)
            {
                var existingItem = order.Items
                    .FirstOrDefault(i => i.MenuItemId == incomingItem.MenuItemId);

                if (existingItem != null)
                {
                    // Update quantity
                    existingItem.Quantity = incomingItem.Quantity;
                }
                else
                {
                    // Add new MenuItem to order
                    order.Items.Add(new OrderItem
                    {
                        MenuItemId = incomingItem.MenuItemId,
                        Quantity = incomingItem.Quantity,
                        UnitPrice = menuItems[incomingItem.MenuItemId].Price
                    });
                }
            }

            // --- REMOVE DELETED ITEMS ---
            var incomingMenuItemIds = dto.Items.Select(i => i.MenuItemId).ToHashSet();
            var itemsToRemove = order.Items
                .Where(i => !incomingMenuItemIds.Contains(i.MenuItemId))
                .ToList();

            _dbContext.OrderItems.RemoveRange(itemsToRemove);

            // Recalculate total amount
            order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

            // Save changes
            await _dbContext.SaveChangesAsync();

            // Reload and map using helper method
            var updatedOrder = await _dbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .Where(o => o.Id == order.Id)
                .Select(OrderMappingHelper.OrderToDto)
                .FirstOrDefaultAsync();

            if (updatedOrder is null)
                throw new InvalidOperationException($"Order {order.Id} was updated but could not be reloaded.");

            var orderCard = OrderMappingHelper.ToOrderCardDto(updatedOrder);

            if (statusChanged)
            {
                await _notificationPublisher.OrderStatusChangedAsync(new OrderStatusChangedDto
                {
                    OrderId = updatedOrder.Id,
                    OldStatus = oldStatus,
                    NewStatus = updatedOrder.Status,
                    ChangedAt = updatedOrder.StatusChangedAt,
                    Order = orderCard
                }, updatedOrder.UserId);
            }
            else
            {
                await _notificationPublisher.OrderUpdatedAsync(orderCard);
            }

            return updatedOrder;
        }

        public async Task<OrderDto> ChangeOrderStatusAsync(int id, UpdateOrderStatusDto dto)
        {
            if (!dto.Status.HasValue)
                throw new ValidationException("Order status is required");

            var order = await _dbContext.Orders.FindAsync(id);

            if (order == null)
                throw new NotFoundException($"Order {id} not found");

            var oldStatus = order.Status;
            var statusChanged = ApplyStatusTransition(order, dto.Status.Value);

            if (statusChanged)
                await _dbContext.SaveChangesAsync();

            var updatedOrder = await _dbContext.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                    .ThenInclude(i => i.MenuItem)
                .Where(o => o.Id == order.Id)
                .Select(OrderMappingHelper.OrderToDto)
                .FirstOrDefaultAsync();

            if (updatedOrder is null)
                throw new InvalidOperationException($"Order {order.Id} was updated but could not be reloaded.");

            if (statusChanged)
            {
                await _notificationPublisher.OrderStatusChangedAsync(new OrderStatusChangedDto
                {
                    OrderId = updatedOrder.Id,
                    OldStatus = oldStatus,
                    NewStatus = updatedOrder.Status,
                    ChangedAt = updatedOrder.StatusChangedAt,
                    Order = OrderMappingHelper.ToOrderCardDto(updatedOrder)
                }, updatedOrder.UserId);
            }

            return updatedOrder;
        }

        // DeleteOrder
        public async Task DeleteOrderAsync(int id)
        {
            var order = await _dbContext.Orders.FindAsync(id);

            if (order == null)
                throw new NotFoundException($"Order {id} not found");

            if (order.Status != OrderStatus.Cancelled)
            {
                throw new ConflictException(
                    $"Order {id} must be cancelled before it can be deleted");
            }

            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
        }

        private static bool ApplyStatusTransition(Order order, OrderStatus newStatus)
        {
            if (order.Status == newStatus)
                return false;

            if (!IsValidStatusTransition(order.Status, newStatus))
            {
                throw new ConflictException(
                    $"Order status cannot be changed from {order.Status} to {newStatus}");
            }

            var changedAt = DateTime.UtcNow;
            order.Status = newStatus;
            order.StatusChangedAt = changedAt;

            switch (newStatus)
            {
                case OrderStatus.Confirmed:
                    order.ConfirmedAt ??= changedAt;
                    break;
                case OrderStatus.Preparing:
                    order.PreparingAt ??= changedAt;
                    break;
                case OrderStatus.Ready:
                    order.ReadyAt ??= changedAt;
                    break;
                case OrderStatus.Completed:
                    order.CompletedAt ??= changedAt;
                    break;
                case OrderStatus.Cancelled:
                    order.CancelledAt ??= changedAt;
                    break;
            }

            return true;
        }

        private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return currentStatus switch
            {
                OrderStatus.Pending =>
                    newStatus is OrderStatus.Confirmed or OrderStatus.Cancelled,
                OrderStatus.Confirmed =>
                    newStatus is OrderStatus.Preparing or OrderStatus.Cancelled,
                OrderStatus.Preparing =>
                    newStatus is OrderStatus.Ready or OrderStatus.Cancelled,
                OrderStatus.Ready =>
                    newStatus is OrderStatus.Completed or OrderStatus.Cancelled,
                OrderStatus.Completed or OrderStatus.Cancelled => false,
                _ => false
            };
        }
    }
}

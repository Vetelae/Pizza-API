using Pizza_API.Entities.Dtos.Order;

namespace Pizza_API.Services
{
    public interface IOrderAdminService
    {
        Task<List<OrderCardDto>> GetActiveOrdersAsync();
        Task<OrderHistoryResponseDto> GetOrderHistoryAsync(
            OrderHistoryQueryDto query,
            CancellationToken cancellationToken = default);
        Task<OrderDto> GetOrderByIdForAdminAsync(int id);
        Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto dto);
        Task<OrderDto> ChangeOrderStatusAsync(int id, UpdateOrderStatusDto dto);
        Task DeleteOrderAsync(int id);
    }
}

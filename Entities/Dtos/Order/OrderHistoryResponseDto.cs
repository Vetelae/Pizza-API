namespace Pizza_API.Entities.Dtos.Order
{
    public class OrderHistoryResponseDto
    {
        public List<OrderHistoryItemDto> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}

namespace Pizza_API.Entities.Dtos.MenuItem
{
    public class CreateMenuItemDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
    }
}
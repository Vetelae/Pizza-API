namespace Pizza_API.Entities.Dtos.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public string? ImageFileName { get; set; }
    }
}
namespace Pizza_API.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public string? ImagePath { get; set; }
        public string? ImageFileName { get; set; }

        public ICollection<MenuItem> MenuItems { get; set; }
            = new List<MenuItem>();
    }
}
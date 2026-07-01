namespace Pizza_API.Entities.Dtos.News
{
    public class NewsDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
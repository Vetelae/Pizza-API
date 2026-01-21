namespace Pizza_API.Entities.Dtos.News
{
    public class CreateNewsDto
    {
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}

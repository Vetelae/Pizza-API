using System.ComponentModel.DataAnnotations;
using Pizza_API.Constants;

namespace Pizza_API.Entities.Dtos.News
{
    public class CreateNewsDto
    {
        public DateTime Date { get; set; }

        [Required]
        [StringLength(NewsConstraints.TitleMaxLength, MinimumLength = NewsConstraints.TitleMinLength)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(NewsConstraints.ContentMaxLength, MinimumLength = NewsConstraints.ContentMinLength)]
        public string Content { get; set; } = string.Empty;
    }
}
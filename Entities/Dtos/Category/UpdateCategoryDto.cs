using System.ComponentModel.DataAnnotations;
using Pizza_API.Constants;

namespace Pizza_API.Entities.Dtos.Category
{
    public class UpdateCategoryDto
    {
        [Required]
        [StringLength(CategoryConstraints.NameMaxLength, MinimumLength = CategoryConstraints.NameMinLength)]
        public string Name { get; set; } = string.Empty;
    }
}
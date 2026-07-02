using System.ComponentModel.DataAnnotations;
using Pizza_API.Constants;

namespace Pizza_API.Entities.Dtos.MenuItem
{
    public class UpdateMenuItemDto
    {
        [Required]
        [StringLength(MenuItemConstraints.NameMaxLength, MinimumLength = MenuItemConstraints.NameMinLength)]
        public string Name { get; set; } = string.Empty;

        [StringLength(MenuItemConstraints.DescriptionMaxLength)]
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Pizza_API.Entities.Dtos.CartItem
{
    public class UpdateCartItemDto
    {
        [Required]
        [Range(1, 99, ErrorMessage = "Quantity must be between 1 and 99")]
        public int Quantity { get; set; }

        [MaxLength(200)]
        public string? Notes { get; set; }
    }
}
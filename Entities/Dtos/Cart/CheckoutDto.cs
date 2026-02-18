using System.ComponentModel.DataAnnotations;

namespace Pizza_API.Entities.Dtos.Cart
{
    public class CheckoutDto
    {
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string CustomerEmail { get; set; } = null!;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string CustomerPhone { get; set; } = null!;

        // Only required for delivery
        [MaxLength(200)]
        public string? DeliveryAddress { get; set; }

        [Required]
        public OrderType Type { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = null!; // "cash" or "card"

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using Pizza_API.Constants;
using Pizza_API.Entities.Dtos.OrderItem;
using Pizza_API.Enums;

namespace Pizza_API.Entities.Dtos.Order
{
    public class UpdateOrderDto
    {
        // Customer info
        [Required]
        [StringLength(OrderConstraints.NameMaxLength, MinimumLength = OrderConstraints.NameMinLength)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(OrderConstraints.EmailMaxLength, MinimumLength = OrderConstraints.EmailMinLength)]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(OrderConstraints.PhoneMaxLength, MinimumLength = OrderConstraints.PhoneMinLength)]
        public string CustomerPhone { get; set; } = string.Empty;

        [StringLength(OrderConstraints.AddressMaxLength, MinimumLength = OrderConstraints.AddressMinLength)]
        public string? DeliveryAddress { get; set; }

        // Order details
        [EnumDataType(typeof(OrderType))]
        public OrderType Type { get; set; }

        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus Status { get; set; }

        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(OrderConstraints.NotesMaxLength)]
        public string? Notes { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type == OrderType.Delivery && string.IsNullOrWhiteSpace(DeliveryAddress))
            {
                yield return new ValidationResult(
                    "Delivery address is required for delivery orders.",
                    new[] { nameof(DeliveryAddress) });
            }
        }
    }
}
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Pizza_API.Constants;

namespace Pizza_API.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(AuthConstraints.NameMaxLength, MinimumLength = AuthConstraints.NameMinLength)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(AuthConstraints.NameMaxLength, MinimumLength = AuthConstraints.NameMinLength)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(OrderConstraints.AddressMaxLength, MinimumLength = OrderConstraints.AddressMinLength)]
        public string? Address { get; set; }

        [StringLength(OrderConstraints.PhoneMaxLength, MinimumLength = OrderConstraints.PhoneMinLength)]
        public override string? PhoneNumber { get; set; }

        public DateTimeOffset? FailedLoginWindowStartUtc { get; set; }
        public int LockoutLevel { get; set; }
        public DateTimeOffset? LastLockoutAtUtc { get; set; }

        // Navigation properties
        public Cart? Cart { get; set; }  // One-to-one: user's active cart
        public ICollection<Order> Orders { get; set; } = new List<Order>();  // Order history
    }
}

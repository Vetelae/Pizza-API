using System.ComponentModel.DataAnnotations;
using Pizza_API.Constants;

namespace Pizza_API.Entities.Dtos.Auth
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(AuthConstraints.PasswordMaxLength, MinimumLength = AuthConstraints.PasswordMinLength)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(AuthConstraints.NameMaxLength, MinimumLength = AuthConstraints.NameMinLength)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(AuthConstraints.NameMaxLength, MinimumLength = AuthConstraints.NameMinLength)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(OrderConstraints.AddressMaxLength, MinimumLength = OrderConstraints.AddressMinLength)]
        public string? Address { get; set; }

        [StringLength(OrderConstraints.PhoneMaxLength, MinimumLength = OrderConstraints.PhoneMinLength)]
        public string? PhoneNumber { get; set; }
    }
}

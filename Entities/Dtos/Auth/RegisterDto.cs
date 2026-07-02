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
    }
}
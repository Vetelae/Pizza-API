using System.ComponentModel.DataAnnotations;
using Pizza_API.Constants;

namespace Pizza_API.Entities.Dtos.Auth
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [StringLength(AuthConstraints.PasswordMaxLength, MinimumLength = AuthConstraints.PasswordMinLength)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
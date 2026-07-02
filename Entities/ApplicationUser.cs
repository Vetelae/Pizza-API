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

        // Navigation properties
        public Cart? Cart { get; set; }  // One-to-one: user's active cart
        public ICollection<Order> Orders { get; set; } = new List<Order>();  // Order history
    }
}
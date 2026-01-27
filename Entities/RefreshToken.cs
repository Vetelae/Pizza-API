using System.ComponentModel.DataAnnotations;

namespace Pizza_API.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Token { get; set; } = default!;

        [Required]
        public string UserId { get; set; } = default!;

        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresOnUtc { get; set; }

        public bool IsRevoked { get; set; }
        public DateTime? RevokedOnUtc { get; set; }

        public ApplicationUser User { get; set; } = default!;
    }
}
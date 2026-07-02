using System.ComponentModel.DataAnnotations;

namespace Pizza_API.Entities.Dtos.Auth
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
namespace Pizza_API.Entities.Dtos.Auth
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Role { get; set; }
    }
}
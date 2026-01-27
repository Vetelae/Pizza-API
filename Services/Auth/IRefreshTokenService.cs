
namespace Pizza_API.Services
{
    public interface IRefreshTokenService
    {
        Task<(string JwtToken, string RefreshToken)?> ValidateAndRotateAsync(string refreshToken);
        Task<string> GenerateRefreshTokenAsync(string userId);
        Task RevokeTokenAsync(string token);
    }
}
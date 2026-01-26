using Pizza_API.Entities;

namespace Pizza_API.Services
{
    public interface IJwtTokenService
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser user);
    }
}
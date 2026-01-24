using Pizza_API.Entities.Dtos.Auth;

namespace Pizza_API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    }
}
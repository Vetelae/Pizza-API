using Pizza_API.Entities.Dtos.Auth;

namespace Pizza_API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> ConfirmEmailAsync(string userId, string token);
        Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<UserProfileDto?> GetUserProfileAsync(string userId);
        Task<UserProfileDto> UpdateUserProfileAsync(string userId, UpdateUserProfileDto dto);
    }
}

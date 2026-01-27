using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Auth;

namespace Pizza_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        // RegisterAsync
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User with this email already exists"
                };
            }

            // Create new user
            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // Generate JWT token
            var token = await _jwtTokenService.GenerateAccessTokenAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully",
                UserId = user.Id,
                Token = token
            };
        }

        // LoginAsync
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Find user
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Check lockout
            if (await _userManager.IsLockedOutAsync(user))
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                var remaining = lockoutEnd?.Subtract(DateTimeOffset.UtcNow).TotalSeconds ?? 0;

                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Account is locked. Try again in {Math.Ceiling(remaining)} seconds."
                };
            }

            // Check password
            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                await _userManager.AccessFailedAsync(user);

                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Success. Reset failed count and generate token
            await _userManager.ResetAccessFailedCountAsync(user);
            var token = await _jwtTokenService.GenerateAccessTokenAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                UserId = user.Id,
                Token = token
            };
        }
    }
}
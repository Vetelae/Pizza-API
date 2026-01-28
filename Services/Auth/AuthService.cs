using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Auth;

namespace Pizza_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IConfiguration _configuration;


        public AuthService(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IEmailSenderService emailSenderService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _emailSenderService = emailSenderService;
            _configuration = configuration;
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
                LastName = registerDto.LastName,
                EmailConfirmed = false
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

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // Build confirmation link
            var frontendUrl = _configuration["Frontend:Url"] ?? "http://localhost:3000";
            var confirmationLink = $"{frontendUrl}/confirm-email?userId={user.Id}&token={encodedToken}";

            // Send confirmation email
            var emailSubject = "Confirm your email";
            var emailBody = $@"
                 <h2>Welcome to Pizza Shop!</h2>
                 <p>Please confirm your email address by clicking the link below:</p>
                 <p><a href='{confirmationLink}'>Confirm Email</a></p>
                 <p>If you didn't create this account, you can safely ignore this email.</p>
            ";

            try
            {
                await _emailSenderService.SendEmailAsync(user.Email, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Failed to send confirmation email: {ex.Message}");
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully. Please check your email to confirm your account.",
                UserId = user.Id,
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

            // Check if email is confirmed
            if (!user.EmailConfirmed)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Please confirm your email before logging in. Check your inbox."
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

        // ConfirmEmailAsync
        public async Task<AuthResponseDto> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            if (user.EmailConfirmed)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email already confirmed"
                };
            }

            // Decode token
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid or expired confirmation token"
                };
            }

            // Generate tokens for immediate login
            var jwtToken = await _jwtTokenService.GenerateAccessTokenAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Email confirmed successfully. You are now logged in.",
                UserId = user.Id,
                Token = jwtToken
            };
        }
    }
}
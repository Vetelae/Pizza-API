using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.Auth;
using Pizza_API.Services;

namespace Pizza_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthController(IAuthService authService, IRefreshTokenService refreshTokenService)
        {
            _authService = authService;
            _refreshTokenService = refreshTokenService;
        }

        // POST: Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // POST: Confirm Email
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "User ID and token are required" });
            }

            var result = await _authService.ConfirmEmailAsync(userId, token);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            // Generate refresh token after email confirmation
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(result.UserId);
            result.RefreshToken = refreshToken;

            return Ok(result);
        }

        // POST: Login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            // Generate refresh token
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(result.UserId);

            // Attach refresh token to response
            result.RefreshToken = refreshToken;

            return Ok(result);
        }

        // GET: Me
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var profile = await _authService.GetUserProfileAsync(userId);

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        // PUT: Me
        [Authorize]
        [HttpPut("me")]
        public async Task<ActionResult<UserProfileDto>> UpdateMe([FromBody] UpdateUserProfileDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException();
            }

            var profile = await _authService.UpdateUserProfileAsync(userId, dto);

            return Ok(profile);
        }

        // POST: Forgot password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);

            // Always return 200 OK to prevent email enumeration
            return Ok(result);
        }

        // POST: Reset password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(resetPasswordDto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // POST: Refresh
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshTokenRequestDto refreshTokenRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(refreshTokenRequest.RefreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            var tokens = await _refreshTokenService.ValidateAndRotateAsync(refreshTokenRequest.RefreshToken);

            if (tokens == null)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            // Extract userId from the JWT token or get it from the refresh token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokens.Value.JwtToken);
            var userId = jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var email = jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            var role = jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value;

            return Ok(new AuthResponseDto
            {
                Success = true,
                Message = "Token refreshed successfully",
                UserId = userId,
                Email = email,
                Role = role,
                Token = tokens.Value.JwtToken,
                RefreshToken = tokens.Value.RefreshToken
            });
        }

        // POST: Logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto refreshTokenRequest)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenRequest.RefreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            await _refreshTokenService.RevokeTokenAsync(refreshTokenRequest.RefreshToken);
            return Ok(new { message = "Logged out successfully" });
        }
    }
}

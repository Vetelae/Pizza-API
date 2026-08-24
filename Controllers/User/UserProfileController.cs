using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.Auth;
using Pizza_API.Services;

namespace Pizza_API.Controllers.User
{
    [Route("api/user/me")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UserProfileController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: api/user/me
        [HttpGet]
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

        // PUT: api/user/me
        [HttpPut]
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
    }
}

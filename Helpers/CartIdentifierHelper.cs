using System.Security.Claims;

namespace Pizza_API.Helpers
{
    public static class CartIdentifierHelper
    {
        public static (string? userId, string? sessionId) GetCartIdentifiers(HttpContext context)
        {
            var userId = context.User.Identity?.IsAuthenticated == true
                ? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;

            var sessionId = context.Request.Headers["X-Session-Id"].FirstOrDefault();

            return (userId, sessionId);
        }
    }
}
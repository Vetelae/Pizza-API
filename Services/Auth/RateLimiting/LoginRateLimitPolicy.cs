using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Pizza_API.Entities.Dtos.Auth;
using Pizza_API.Options;

namespace Pizza_API.Services
{
    internal sealed class LoginRateLimitPolicy : IRateLimiterPolicy<string>
    {
        private readonly LoginSecurityOptions _options;
        private readonly ILogger<LoginRateLimitPolicy> _logger;

        public LoginRateLimitPolicy(
            IOptions<LoginSecurityOptions> options,
            ILogger<LoginRateLimitPolicy> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public Func<OnRejectedContext, CancellationToken, ValueTask> OnRejected => HandleRejectedAsync;

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            return RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey,
                _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = _options.IpPermitLimit,
                    Window = TimeSpan.FromSeconds(_options.IpWindowSeconds),
                    SegmentsPerWindow = _options.IpSegmentsPerWindow,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
        }

        private async ValueTask HandleRejectedAsync(
            OnRejectedContext context,
            CancellationToken cancellationToken)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

            var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var estimatedRetryAfter)
                ? estimatedRetryAfter
                : TimeSpan.FromSeconds(_options.IpWindowSeconds);

            context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds)
                .ToString(CultureInfo.InvariantCulture);

            _logger.LogWarning(
                "Login IP rate limit rejected request from {RemoteIpAddress}",
                context.HttpContext.Connection.RemoteIpAddress);

            await context.HttpContext.Response.WriteAsJsonAsync(
                new AuthResponseDto
                {
                    Success = false,
                    Message = "Too many login attempts, try again later."
                },
                cancellationToken);
        }
    }
}

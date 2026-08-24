using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pizza_API.Entities.Dtos.Auth;
using Pizza_API.Options;

namespace Pizza_API.Services
{
    internal static class AuthRateLimitPolicyExtensions
    {
        public static RateLimiterOptions AddAuthIpPolicy(
            this RateLimiterOptions options,
            string policyName,
            Func<AuthRateLimitingOptions, AuthRateLimitRuleOptions> ruleSelector)
        {
            return options.AddPolicy(
                policyName,
                httpContext =>
                {
                    var authOptions = httpContext.RequestServices
                        .GetRequiredService<IOptions<AuthRateLimitingOptions>>()
                        .Value;
                    var rule = ruleSelector(authOptions);
                    var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey,
                        _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = rule.PermitLimit,
                            Window = TimeSpan.FromSeconds(rule.WindowSeconds),
                            SegmentsPerWindow = rule.SegmentsPerWindow,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        });
                });
        }

        public static async ValueTask HandleRejectedAsync(
            OnRejectedContext context,
            CancellationToken cancellationToken)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

            var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var estimatedRetryAfter)
                ? estimatedRetryAfter
                : TimeSpan.FromMinutes(1);

            context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds)
                .ToString(CultureInfo.InvariantCulture);

            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("Pizza_API.Services.RateLimiting");
            var endpointName = context.HttpContext.GetEndpoint()?.DisplayName
                ?? context.HttpContext.Request.Path.Value
                ?? "unknown";

            logger.LogWarning(
                "Rate limit rejected request to {EndpointName} from {RemoteIpAddress}",
                endpointName,
                context.HttpContext.Connection.RemoteIpAddress);

            if (context.HttpContext.Request.Path.StartsWithSegments(
                "/api/auth",
                StringComparison.OrdinalIgnoreCase))
            {
                await context.HttpContext.Response.WriteAsJsonAsync(
                    new AuthResponseDto
                    {
                        Success = false,
                        Message = "Too many requests. Please try again later."
                    },
                    cancellationToken);
                return;
            }

            await context.HttpContext.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = "Too Many Requests",
                    Detail = "Too many requests. Please try again later."
                },
                cancellationToken);
        }
    }
}

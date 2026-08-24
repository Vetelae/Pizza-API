using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Pizza_API.Options;

namespace Pizza_API.Services
{
    internal static class TrafficRateLimitPolicyExtensions
    {
        public static PartitionedRateLimiter<HttpContext> CreateGlobalLimiter()
        {
            return PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var rule = GetOptions(httpContext).Global;
                return CreatePartition(GetClientIpPartitionKey(httpContext), rule);
            });
        }

        public static RateLimiterOptions AddTrafficIpPolicy(
            this RateLimiterOptions options,
            string policyName,
            Func<TrafficRateLimitingOptions, TrafficRateLimitRuleOptions> ruleSelector)
        {
            return options.AddPolicy(
                policyName,
                httpContext => CreatePartition(
                    GetClientIpPartitionKey(httpContext),
                    ruleSelector(GetOptions(httpContext))));
        }

        public static RateLimiterOptions AddCartPolicy(
            this RateLimiterOptions options,
            string policyName,
            Func<TrafficRateLimitingOptions, TrafficRateLimitRuleOptions> ruleSelector)
        {
            return options.AddPolicy(
                policyName,
                httpContext => CreatePartition(
                    GetCartPartitionKey(httpContext),
                    ruleSelector(GetOptions(httpContext))));
        }

        private static RateLimitPartition<string> CreatePartition(
            string partitionKey,
            TrafficRateLimitRuleOptions rule)
        {
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
        }

        private static TrafficRateLimitingOptions GetOptions(HttpContext httpContext)
        {
            return httpContext.RequestServices
                .GetRequiredService<IOptions<TrafficRateLimitingOptions>>()
                .Value;
        }

        private static string GetCartPartitionKey(HttpContext httpContext)
        {
            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrWhiteSpace(userId))
                    return $"user:{userId}";
            }

            return GetClientIpPartitionKey(httpContext);
        }

        private static string GetClientIpPartitionKey(HttpContext httpContext)
        {
            var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
            return remoteIpAddress == null
                ? "ip:unknown"
                : $"ip:{remoteIpAddress.MapToIPv6()}";
        }
    }
}

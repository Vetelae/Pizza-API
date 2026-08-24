namespace Pizza_API.Options
{
    public class AuthRateLimitingOptions
    {
        public const string SectionName = "AuthRateLimiting";

        public AuthRateLimitRuleOptions Register { get; set; } = new()
        {
            PermitLimit = 5,
            WindowSeconds = 600,
            SegmentsPerWindow = 10
        };

        public AuthRateLimitRuleOptions ForgotPassword { get; set; } = new()
        {
            PermitLimit = 5,
            WindowSeconds = 600,
            SegmentsPerWindow = 10
        };

        public AuthRateLimitRuleOptions ResetPassword { get; set; } = new()
        {
            PermitLimit = 10,
            WindowSeconds = 600,
            SegmentsPerWindow = 10
        };

        public AuthRateLimitRuleOptions ConfirmEmail { get; set; } = new()
        {
            PermitLimit = 10,
            WindowSeconds = 600,
            SegmentsPerWindow = 10
        };

        public AuthRateLimitRuleOptions Refresh { get; set; } = new()
        {
            PermitLimit = 30,
            WindowSeconds = 60,
            SegmentsPerWindow = 6
        };

        public AuthRateLimitRuleOptions Logout { get; set; } = new()
        {
            PermitLimit = 30,
            WindowSeconds = 60,
            SegmentsPerWindow = 6
        };

        public int ForgotPasswordAccountCooldownMinutes { get; set; } = 15;
    }

    public class AuthRateLimitRuleOptions
    {
        public int PermitLimit { get; set; }
        public int WindowSeconds { get; set; }
        public int SegmentsPerWindow { get; set; }
    }
}

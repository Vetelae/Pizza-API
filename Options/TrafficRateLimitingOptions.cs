namespace Pizza_API.Options
{
    public class TrafficRateLimitingOptions
    {
        public const string SectionName = "TrafficRateLimiting";

        public TrafficRateLimitRuleOptions Global { get; set; } = new()
        {
            PermitLimit = 600,
            WindowSeconds = 60,
            SegmentsPerWindow = 6
        };

        public TrafficRateLimitRuleOptions PublicReads { get; set; } = new()
        {
            PermitLimit = 240,
            WindowSeconds = 60,
            SegmentsPerWindow = 6
        };

        public TrafficRateLimitRuleOptions CartReads { get; set; } = new()
        {
            PermitLimit = 60,
            WindowSeconds = 60,
            SegmentsPerWindow = 6
        };

        public TrafficRateLimitRuleOptions CartMutations { get; set; } = new()
        {
            PermitLimit = 30,
            WindowSeconds = 60,
            SegmentsPerWindow = 6
        };

        public TrafficRateLimitRuleOptions Checkout { get; set; } = new()
        {
            PermitLimit = 5,
            WindowSeconds = 600,
            SegmentsPerWindow = 10
        };
    }

    public class TrafficRateLimitRuleOptions
    {
        public int PermitLimit { get; set; }
        public int WindowSeconds { get; set; }
        public int SegmentsPerWindow { get; set; }
    }
}

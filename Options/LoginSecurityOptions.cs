namespace Pizza_API.Options
{
    public class LoginSecurityOptions
    {
        public const string SectionName = "LoginSecurity";

        public int IpPermitLimit { get; set; } = 20;
        public int IpWindowSeconds { get; set; } = 60;
        public int IpSegmentsPerWindow { get; set; } = 6;
        public int AccountMaxFailedAttempts { get; set; } = 5;
        public int AccountObservationWindowMinutes { get; set; } = 5;
        public int[] LockoutDurationsMinutes { get; set; } = [];
        public int EscalationResetHours { get; set; } = 24;
    }
}

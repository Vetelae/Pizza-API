namespace Pizza_API.Helpers
{
    public static class TimeZoneHelper
    {
        public static TimeZoneInfo FindSystemTimeZoneById(string timeZoneId)
        {
            if (string.IsNullOrWhiteSpace(timeZoneId))
                throw new TimeZoneNotFoundException("A time zone ID is required.");

            if (TryFindSystemTimeZoneById(timeZoneId, out var timeZone))
                return timeZone;

            var converted = OperatingSystem.IsWindows()
                ? TimeZoneInfo.TryConvertIanaIdToWindowsId(
                    timeZoneId,
                    out var alternativeTimeZoneId)
                : TimeZoneInfo.TryConvertWindowsIdToIanaId(
                    timeZoneId,
                    out alternativeTimeZoneId);

            if (converted
                && TryFindSystemTimeZoneById(
                    alternativeTimeZoneId!,
                    out timeZone))
            {
                return timeZone;
            }

            throw new TimeZoneNotFoundException(
                $"The time zone ID '{timeZoneId}' was not found.");
        }

        public static bool IsValidSystemTimeZoneId(string timeZoneId)
        {
            try
            {
                FindSystemTimeZoneById(timeZoneId);
                return true;
            }
            catch (TimeZoneNotFoundException)
            {
                return false;
            }
            catch (InvalidTimeZoneException)
            {
                return false;
            }
        }

        private static bool TryFindSystemTimeZoneById(
            string timeZoneId,
            out TimeZoneInfo timeZone)
        {
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return true;
            }
            catch (TimeZoneNotFoundException)
            {
                timeZone = null!;
                return false;
            }
            catch (InvalidTimeZoneException)
            {
                timeZone = null!;
                return false;
            }
        }
    }
}

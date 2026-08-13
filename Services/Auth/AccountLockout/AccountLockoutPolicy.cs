using Microsoft.Extensions.Options;
using Pizza_API.Entities;
using Pizza_API.Options;

namespace Pizza_API.Services
{
    internal sealed class AccountLockoutPolicy
    {
        private readonly LoginSecurityOptions _options;

        public AccountLockoutPolicy(IOptions<LoginSecurityOptions> options)
        {
            _options = options.Value;
        }

        public bool ApplyMaintenance(ApplicationUser user, DateTimeOffset now)
        {
            var changed = false;

            if (user.LockoutEnd <= now)
            {
                user.LockoutEnd = null;
                changed = true;
            }

            if (user.LastLockoutAtUtc.HasValue
                && now - user.LastLockoutAtUtc.Value >= TimeSpan.FromHours(_options.EscalationResetHours))
            {
                user.LockoutLevel = 0;
                user.LastLockoutAtUtc = null;
                changed = true;
            }

            return changed;
        }

        public bool IsLockedOut(ApplicationUser user, DateTimeOffset now)
        {
            return user.LockoutEnabled && user.LockoutEnd > now;
        }

        public AccountFailureResult RecordFailure(ApplicationUser user, DateTimeOffset now)
        {
            ApplyMaintenance(user, now);

            if (!user.LockoutEnabled || IsLockedOut(user, now))
            {
                return new AccountFailureResult(false, null);
            }

            var observationWindow = TimeSpan.FromMinutes(_options.AccountObservationWindowMinutes);

            if (!user.FailedLoginWindowStartUtc.HasValue
                || now - user.FailedLoginWindowStartUtc.Value >= observationWindow)
            {
                user.FailedLoginWindowStartUtc = now;
                user.AccessFailedCount = 1;
            }
            else
            {
                user.AccessFailedCount++;
            }

            if (user.AccessFailedCount < _options.AccountMaxFailedAttempts)
            {
                return new AccountFailureResult(false, null);
            }

            var nextLevel = Math.Min(user.LockoutLevel + 1, _options.LockoutDurationsMinutes.Length);
            var duration = TimeSpan.FromMinutes(_options.LockoutDurationsMinutes[nextLevel - 1]);

            user.AccessFailedCount = 0;
            user.FailedLoginWindowStartUtc = null;
            user.LockoutLevel = nextLevel;
            user.LastLockoutAtUtc = now;
            user.LockoutEnd = now.Add(duration);

            return new AccountFailureResult(true, duration);
        }

        public bool Reset(ApplicationUser user)
        {
            if (user.AccessFailedCount == 0
                && !user.FailedLoginWindowStartUtc.HasValue
                && user.LockoutLevel == 0
                && !user.LastLockoutAtUtc.HasValue
                && !user.LockoutEnd.HasValue)
            {
                return false;
            }

            user.AccessFailedCount = 0;
            user.FailedLoginWindowStartUtc = null;
            user.LockoutLevel = 0;
            user.LastLockoutAtUtc = null;
            user.LockoutEnd = null;

            return true;
        }
    }

    internal readonly record struct AccountFailureResult(
        bool LockoutTriggered,
        TimeSpan? LockoutDuration);
}

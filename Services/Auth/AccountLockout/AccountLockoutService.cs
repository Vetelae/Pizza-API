using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities;

namespace Pizza_API.Services
{
    internal sealed class AccountLockoutService : IAccountLockoutService
    {
        private const int MaxConcurrencyRetries = 3;

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AccountLockoutPolicy _policy;
        private readonly AccountLockoutCoordinator _coordinator;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<AccountLockoutService> _logger;

        public AccountLockoutService(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            AccountLockoutPolicy policy,
            AccountLockoutCoordinator coordinator,
            TimeProvider timeProvider,
            ILogger<AccountLockoutService> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _policy = policy;
            _coordinator = coordinator;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task<bool> IsLockedOutAsync(string userId)
        {
            using var lockHandle = await _coordinator.AcquireAsync(userId);

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var user = await LoadFreshUserAsync(userId);
                if (user == null)
                {
                    return false;
                }

                var now = _timeProvider.GetUtcNow();
                var changed = _policy.ApplyMaintenance(user, now);
                var isLockedOut = _policy.IsLockedOut(user, now);

                if (!changed || await TryUpdateAsync(user, attempt))
                {
                    return isLockedOut;
                }
            }

            throw CreatePersistenceException();
        }

        public async Task RecordFailureAsync(string userId)
        {
            using var lockHandle = await _coordinator.AcquireAsync(userId);

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var user = await LoadFreshUserAsync(userId);
                if (user == null)
                {
                    return;
                }

                var result = _policy.RecordFailure(user, _timeProvider.GetUtcNow());

                if (!await TryUpdateAsync(user, attempt))
                {
                    continue;
                }

                if (result.LockoutTriggered)
                {
                    _logger.LogWarning(
                        "Account lockout triggered for user {UserId} at level {LockoutLevel} for {LockoutMinutes} minutes",
                        user.Id,
                        user.LockoutLevel,
                        result.LockoutDuration?.TotalMinutes);
                }

                return;
            }

            throw CreatePersistenceException();
        }

        public async Task ResetAsync(string userId)
        {
            using var lockHandle = await _coordinator.AcquireAsync(userId);

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var user = await LoadFreshUserAsync(userId);
                if (user == null || !_policy.Reset(user))
                {
                    return;
                }

                if (await TryUpdateAsync(user, attempt))
                {
                    return;
                }
            }

            throw CreatePersistenceException();
        }

        private async Task<ApplicationUser?> LoadFreshUserAsync(string userId)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(user => user.Id == userId);

            if (user != null)
            {
                await _dbContext.Entry(user).ReloadAsync();
            }

            return user;
        }

        private async Task<bool> TryUpdateAsync(ApplicationUser user, int attempt)
        {
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return true;
            }

            var isConcurrencyFailure = result.Errors.Any(error => error.Code == "ConcurrencyFailure");
            if (isConcurrencyFailure && attempt < MaxConcurrencyRetries)
            {
                return false;
            }

            var errorCodes = string.Join(", ", result.Errors.Select(error => error.Code));
            _logger.LogError(
                "Failed to persist account lockout state for user {UserId}. Identity errors: {ErrorCodes}",
                user.Id,
                errorCodes);

            throw CreatePersistenceException();
        }

        private static InvalidOperationException CreatePersistenceException()
        {
            return new InvalidOperationException("Could not update account lockout state.");
        }
    }
}

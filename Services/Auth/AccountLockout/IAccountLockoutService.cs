namespace Pizza_API.Services
{
    public interface IAccountLockoutService
    {
        Task<bool> IsLockedOutAsync(string userId);
        Task RecordFailureAsync(string userId);
        Task ResetAsync(string userId);
    }
}

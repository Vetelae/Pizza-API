namespace Pizza_API.Services
{
    public interface ICustomerOrderSubscriptionAuthorizer
    {
        Task<bool> CanSubscribeAsync(int orderId, string lookupToken);
    }
}

namespace OrderPipelineMiddleware.Services.Interfaces
{
    public interface INotificationService
    {
        Task<bool> NotifyCustomerAsync(string customerId, string message);
    }
}

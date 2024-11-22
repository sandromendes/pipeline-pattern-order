using OrderPipelineMiddleware.Services.Interfaces;

namespace OrderPipelineMiddleware.Services
{
    public class NotificationService : INotificationService
    {
        public async Task<bool> NotifyCustomerAsync(string customerId, string message)
        {
            Console.WriteLine($"Sending notification to Customer ID: {customerId}...");
            
            await Task.Delay(500); // Simula o tempo de notificação ao cliente
            
            Console.WriteLine($"Notification sent successfully: {message}");
            return true;
        }
    }

}

using OrderPipelineMiddleware.Services.Interfaces;

namespace OrderPipelineMiddleware.Services
{
    public class PaymentService : IPaymentService
    {
        public async Task<bool> ProcessPaymentAsync(double amount)
        {
            Console.WriteLine($"Processing payment of ${amount}...");
            
            await Task.Delay(500); // Simula o tempo de espera para a realização do pagamento
            
            Console.WriteLine("Payment processed successfully!");
            return true;
        }
    }

}

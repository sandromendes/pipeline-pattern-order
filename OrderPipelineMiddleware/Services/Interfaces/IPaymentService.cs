namespace OrderPipelineMiddleware.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(double amount);
    }
}

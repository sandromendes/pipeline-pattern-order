using OrderPipelineMiddleware.Domain;

namespace OrderPipelineMiddleware.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<Invoice> GenerateInvoiceAsync(Order order);
    }
}

using OrderPipelineMiddleware.Domain;
using OrderPipelineMiddleware.Services.Interfaces;

namespace OrderPipelineMiddleware.Services
{
    public class InvoiceService : IInvoiceService
    {
        public async Task<Invoice> GenerateInvoiceAsync(Order order)
        {
            Console.WriteLine($"Generating invoice for Order ID: {order.OrderId}...");
            await Task.Delay(500); // Simula o tempo de geração da fatura

            var invoice = new Invoice
            {
                InvoiceId = Guid.NewGuid().ToString(),
                OrderId = order.OrderId,
                Amount = order.TotalAmount,
                GeneratedDate = DateTime.UtcNow
            };

            Console.WriteLine($"Invoice generated successfully: {invoice.InvoiceId}");
            return invoice;
        }
    }

}

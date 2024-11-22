using OrderPipelineMiddleware.Domain;
using OrderPipelineMiddleware.Middleware;
using OrderPipelineMiddleware.Pipeline;
using OrderPipelineMiddleware.Services;
using OrderPipelineMiddleware.Steps;

class Program
{
    static async Task Main(string[] args)
    {
        var paymentService = new PaymentService();
        var invoiceService = new InvoiceService();
        var notificationService = new NotificationService();

        var order = new Order
        {
            OrderId = "ORDER123",
            CustomerId = "CUSTOMER001",
            Items = new List<OrderItem>
            {
                new OrderItem { ProductId = "P1", Quantity = 1 }
            },
            TotalAmount = 250.0
        };

        var processedOrder = await PipelineBuilder<Order>
            .Create()
            .UseMiddleware(ExceptionHandlingMiddleware<Order>.Apply)
            .AddStep(new ValidateOrderStep())
            .AddStep(new CalculateShippingStep())
            .AddStep(new ReserveStockStep())
            .AddStep(new ProcessPaymentStep(paymentService))
            .AddStep(new GenerateInvoiceStep(invoiceService))
            .AddStep(new NotifyCustomerStep(notificationService))
            .ProcessAsync(order);

        Console.WriteLine("Pipeline ended succefully!");
    }
}

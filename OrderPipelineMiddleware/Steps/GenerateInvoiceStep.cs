using OrderPipelineMiddleware.Domain;
using OrderPipelineMiddleware.Services.Interfaces;

namespace OrderPipelineMiddleware.Steps
{
    public class GenerateInvoiceStep : BaseStep<Order>
    {
        private readonly IInvoiceService _invoiceService;

        public GenerateInvoiceStep(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        protected override bool EnableRetry => false; // Retry desativado

        protected override async Task<Order> ExecuteAsync(Order input)
        {
            var invoice = await _invoiceService.GenerateInvoiceAsync(input);

            if (invoice == null)
                throw new Exception("Falha ao gerar a nota fiscal.");

            input.InvoiceGenerated = true;
            input.Invoice = invoice;
            return input;
        }
    }

}

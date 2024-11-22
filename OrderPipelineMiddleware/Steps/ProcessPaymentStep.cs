using OrderPipelineMiddleware.Domain;
using OrderPipelineMiddleware.Services.Interfaces;

namespace OrderPipelineMiddleware.Steps
{
    public class ProcessPaymentStep : BaseStep<Order>
    {
        private readonly IPaymentService _paymentService;

        public ProcessPaymentStep(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        protected override bool EnableRetry => true; // Retry ativado
        protected override int MaxRetries => 3; // Até 3 tentativas

        protected override async Task<Order> ExecuteAsync(Order input)
        {
            var paymentResult = await _paymentService.ProcessPaymentAsync(input.TotalAmount);

            if (!paymentResult)
                throw new Exception("Falha ao processar o pagamento.");

            input.PaymentProcessed = true;
            return input;
        }
    }

}

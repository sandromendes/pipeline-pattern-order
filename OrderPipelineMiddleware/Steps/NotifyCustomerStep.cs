using OrderPipelineMiddleware.Domain;
using OrderPipelineMiddleware.Services.Interfaces;

namespace OrderPipelineMiddleware.Steps
{
    public class NotifyCustomerStep : BaseStep<Order>
    {
        private readonly INotificationService _notificationService;

        public NotifyCustomerStep(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        protected override bool EnableRetry => true; // Retry ativado
        protected override int MaxRetries => 2; // Até 2 tentativas

        protected override async Task<Order> ExecuteAsync(Order input)
        {
            bool notificationSent = await _notificationService
                .NotifyCustomerAsync(
                    input.CustomerId,
                    $"Seu pedido #{input.OrderId} foi processado com sucesso!"
                );

            if (!notificationSent)
                throw new Exception("Falha ao notificar o cliente.");

            input.CustomerNotified = true;
            return input;
        }
    }

}

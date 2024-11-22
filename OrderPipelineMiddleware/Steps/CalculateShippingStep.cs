using OrderPipelineMiddleware.Domain;

namespace OrderPipelineMiddleware.Steps
{
    public class CalculateShippingStep : BaseStep<Order>
    {
        protected override bool EnableRetry => false; // Retry não necessário aqui

        protected override Task<Order> ExecuteAsync(Order input)
        {
            // Simula cálculo de frete
            input.ShippingCost = 15.0;
            return Task.FromResult(input);
        }
    }

}

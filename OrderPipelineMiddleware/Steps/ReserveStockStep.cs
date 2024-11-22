using OrderPipelineMiddleware.Domain;

namespace OrderPipelineMiddleware.Steps
{
    public class ReserveStockStep : BaseStep<Order>
    {
        protected override bool EnableRetry => true;
        protected override int MaxRetries => 3; // Retry ativado, até 3 tentativas

        protected override async Task<Order> ExecuteAsync(Order input)
        {
            // Simula erro aleatório para demonstrar retry
            if (new Random().Next(0, 2) == 0)
                throw new Exception("Falha ao reservar estoque!");

            input.Items.ForEach(item => item.Reserved = true);
            return await Task.FromResult(input);
        }
    }

}

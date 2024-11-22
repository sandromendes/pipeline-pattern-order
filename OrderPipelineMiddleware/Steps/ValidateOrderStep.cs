using OrderPipelineMiddleware.Domain;

namespace OrderPipelineMiddleware.Steps
{
    public class ValidateOrderStep : BaseStep<Order>
    {
        protected override bool EnableRetry => false; // Sem retries para validação

        protected override Task<Order> ExecuteAsync(Order input)
        {
            if (string.IsNullOrWhiteSpace(input.CustomerId))
                throw new ArgumentException("Pedido inválido: cliente não identificado.");

            if (input.Items == null || !input.Items.Any())
                throw new ArgumentException("Pedido inválido: sem itens.");

            return Task.FromResult(input);
        }
    }

}

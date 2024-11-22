using OrderPipelineMiddleware.Pipeline;

namespace OrderPipelineMiddleware.Steps
{
    public abstract class BaseStep<T> : IAsyncPipelineStep<T>
    {
        protected virtual int MaxRetries => 1;

        protected virtual bool EnableRetry => false;

        public async Task<T> ProcessAsync(T input)
        {
            int attempts = 0;

            while (true)
            {
                try
                {
                    return await ExecuteAsync(input); // Implementação específica do step
                }
                catch (Exception ex)
                {
                    attempts++;
                    Console.WriteLine($"Erro no step {this.GetType().Name}, tentativa {attempts}/{MaxRetries}: {ex.Message}");

                    if (!EnableRetry || attempts >= MaxRetries)
                    {
                        Console.WriteLine($"Falha crítica no step {this.GetType().Name}.");
                        throw;
                    }
                }
            }
        }

        protected abstract Task<T> ExecuteAsync(T input);
    }

}

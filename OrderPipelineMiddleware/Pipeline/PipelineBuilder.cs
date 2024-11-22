using OrderPipelineMiddleware.Middleware;

namespace OrderPipelineMiddleware.Pipeline
{
    public class PipelineBuilder<T>
    {
        private readonly List<IAsyncPipelineStep<T>> _steps = new();
        private readonly PipelineMiddleware<T> _middleware = new();

        public static PipelineBuilder<T> Create() => new();

        public PipelineBuilder<T> AddStep(IAsyncPipelineStep<T> step)
        {
            _steps.Add(step);
            return this;
        }

        public PipelineBuilder<T> UseMiddleware(Func<MiddlewareDelegate<T>, MiddlewareDelegate<T>> middleware)
        {
            _middleware.Use(middleware);
            return this;
        }

        public async Task<T> ProcessAsync(T input)
        {
            var handler = _middleware.Build(async currentInput =>
            {
                foreach (var step in _steps)
                {
                    currentInput = await step.ProcessAsync(currentInput);
                }
                return currentInput;
            });

            return await handler(input);
        }
    }
}

namespace OrderPipelineMiddleware.Pipeline
{
    public interface IAsyncPipelineStep<T>
    {
        Task<T> ProcessAsync(T input);
    }
}

namespace OrderPipelineMiddleware.Middleware
{
    public class ExceptionHandlingMiddleware<T>
    {
        public static MiddlewareDelegate<T> Apply(MiddlewareDelegate<T> next)
        {
            return async input =>
            {
                try
                {
                    return await next(input);
                }
                catch (Exception ex)
                {
                    // Log da exceção
                    Console.WriteLine($"Erro no pipeline: {ex.Message}");
                    throw; // Relança para análise externa, se necessário
                }
            };
        }
    }
}

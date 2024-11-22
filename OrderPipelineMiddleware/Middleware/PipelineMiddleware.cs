namespace OrderPipelineMiddleware.Middleware
{
    public delegate Task<T> MiddlewareDelegate<T>(T input);

    public class PipelineMiddleware<T>
    {
        private readonly List<Func<MiddlewareDelegate<T>, MiddlewareDelegate<T>>> _middlewares = new();

        public MiddlewareDelegate<T> Build(MiddlewareDelegate<T> finalHandler)
        {
            MiddlewareDelegate<T> handler = finalHandler;

            foreach (var middleware in _middlewares.AsEnumerable().Reverse())
            {
                handler = middleware(handler);
            }

            return handler;
        }

        public void Use(Func<MiddlewareDelegate<T>, MiddlewareDelegate<T>> middleware)
        {
            _middlewares.Add(middleware);
        }
    }

}

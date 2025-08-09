using Skybeam.Abstractions;

namespace Skybeam.WebRoot.Handlers;

public class FooSecondBehavior<TRequest, TResponse>(ILogger<FooSecondBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> HandleAsync(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken ct)
    {
        logger.LogInformation($"Hello from the behavior #2. Decorating {typeof(TRequest).Name}.");
        var response = await next();
        logger.LogInformation("Bye from the behavior #1.");

        return response;
    }
}
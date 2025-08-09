using Skybeam.Abstractions;

namespace Skybeam.WebRoot.Handlers;

public class FooFirstBehavior<TRequest, TResponse>(
    ILogger<FooFirstBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> HandleAsync(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken ct)
    {
        logger.LogInformation($"Hello from the behavior #1. Decorating {typeof(TRequest).Name}.");
        var response = await next();
        logger.LogInformation("Bye from the behavior #1.");

        return response;
    }
}
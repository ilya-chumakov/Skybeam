using Demo.DecoratedHandlers.Abstractions;
using Microsoft.Extensions.Logging;

namespace Demo.DecoratedHandlers.FooDomain;

public class FooFirstBehavior<TRequest, TResponse>(ILogger<FooFirstBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> HandleAsync(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken ct)
    {
        logger.LogInformation($"Decorating {typeof(TRequest).Name}");
        var response = await next();
        logger.LogInformation($"Decorated.");

        return response;
    }
}
using Demo.DecoratedHandlers.Abstractions;
using Microsoft.Extensions.Logging;

namespace Demo.DecoratedHandlers.FooDomain;

public class FooSecondBehavior<TRequest, TResponse>(ILogger<FooSecondBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
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

public class LogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public Task<TResponse> HandleAsync(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken ct = default)
    {
        throw new NotSupportedException();
    }
}
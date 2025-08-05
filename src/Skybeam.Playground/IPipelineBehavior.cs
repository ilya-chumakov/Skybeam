using Microsoft.Extensions.Logging;

namespace Skybeam.Playground;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

public interface IPipelineBehavior<in TRequest, TResponse> where TRequest : notnull
{
    Task<TResponse> Handle(TRequest request, 
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}

public class FirstBehavior<TRequest, TResponse>(ILogger<FirstBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
//where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Decorating {typeof(TRequest).Name}");
        var response = await next();
        logger.LogInformation($"Decorated.");

        return response;
    }
}


public class SecondBehavior<TRequest, TResponse>(ILogger<SecondBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
//where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Decorating {typeof(TRequest).Name}");
        var response = await next();
        logger.LogInformation($"Decorated.");

        return response;
    }
}
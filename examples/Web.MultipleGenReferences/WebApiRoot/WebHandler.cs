using Skybeam.Abstractions;

namespace WebApiRoot;

public record WebQuery;
public record WebResponse;
public class WebQueryHandler(ILogger<WebQueryHandler> logger) 
    : IRequestHandler<WebQuery, WebResponse>
{
    public Task<WebResponse> HandleAsync(WebQuery input, CancellationToken ct = default)
    {
        logger.LogInformation($"{nameof(WebQueryHandler)} is called!");
        return Task.FromResult(new WebResponse());
    }
}

public class WebBehavior<TRequest, TResponse>(ILogger<WebBehavior<TRequest, TResponse>> logger)
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
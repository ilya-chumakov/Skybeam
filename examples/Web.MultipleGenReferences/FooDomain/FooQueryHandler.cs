using Demo.DecoratedHandlers.Abstractions;
using Microsoft.Extensions.Logging;

namespace Demo.DecoratedHandlers.FooDomain;

public class FooQueryHandler(ILogger<FooQueryHandler> logger) 
    : IRequestHandler<FooQuery, FooResponse>
{
    public Task<FooResponse> HandleAsync(FooQuery input, CancellationToken ct = default)
    {
        logger.LogInformation($"{nameof(FooQueryHandler)} is called!");
        return Task.FromResult(new FooResponse());
    }
}
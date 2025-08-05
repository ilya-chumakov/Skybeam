using Demo.DecoratedHandlers.Abstractions;
using Microsoft.Extensions.Logging;

namespace Demo.DecoratedHandlers.Core;

public class CoreQueryHandler(ILogger<CoreQueryHandler> logger) 
    : IRequestHandler<CoreQuery, CoreResponse>
{
    public Task<CoreResponse> HandleAsync(CoreQuery input, CancellationToken ct = default)
    {
        logger.LogInformation($"{nameof(CoreQueryHandler)} is called!");
        return Task.FromResult(new CoreResponse());
    }
}
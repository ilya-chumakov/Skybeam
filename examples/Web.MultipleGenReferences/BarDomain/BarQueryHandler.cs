using Demo.DecoratedHandlers.Abstractions;
using Microsoft.Extensions.Logging;

namespace Demo.DecoratedHandlers.BarDomain;

public class BarQueryHandler(ILogger<BarQueryHandler> logger) 
    : IRequestHandler<BarQuery, BarResponse>
{
    public Task<BarResponse> HandleAsync(BarQuery input, CancellationToken ct = default)
    {
        logger.LogInformation($"{nameof(BarQueryHandler)} is called!");
        return Task.FromResult(new BarResponse());
    }
}
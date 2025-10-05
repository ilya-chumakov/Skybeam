using Microsoft.Extensions.Logging;
using Skybeam.Abstractions;

namespace Core;

public class CoreQueryHandler(ILogger<CoreQueryHandler> logger) 
    : IRequestHandler<CoreQuery, CoreResponse>
{
    public Task<CoreResponse> HandleAsync(CoreQuery input, CancellationToken ct = default)
    {
        logger.LogInformation($"{nameof(CoreQueryHandler)} is called!");
        return Task.FromResult(new CoreResponse());
    }
}
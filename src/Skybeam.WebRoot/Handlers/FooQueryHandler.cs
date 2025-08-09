using Skybeam.Abstractions;

namespace Skybeam.WebRoot.Handlers;

public class FooQueryHandler(ILogger<FooQueryHandler> logger) 
    : IRequestHandler<FooQuery, FooResponse>
{
    public Task<FooResponse> HandleAsync(FooQuery input, CancellationToken ct = default)
    {
        logger.LogInformation($"{nameof(FooQueryHandler)} is called!");
        return Task.FromResult(new FooResponse());
    }
}
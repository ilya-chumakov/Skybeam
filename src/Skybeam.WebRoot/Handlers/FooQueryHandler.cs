using Skybeam.Abstractions;

namespace Skybeam.WebRoot.Handlers;

public class FooQueryHandler(ILogger<FooQueryHandler> logger) 
    : IRequestHandler<FooQuery, FooResponse>
{
    public static readonly string Message = nameof(FooQueryHandler) + " is called.";
    public Task<FooResponse> HandleAsync(FooQuery input, CancellationToken ct = default)
    {
        logger.LogInformation(Message);
        return Task.FromResult(new FooResponse());
    }
}
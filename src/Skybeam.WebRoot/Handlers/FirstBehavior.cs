using Skybeam.Abstractions;

namespace Skybeam.WebRoot.Handlers;

public class FirstBehavior<TRequest, TResponse>(ILogger<FirstBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
{
    public const string BeginMessage = "Begin #1";
    public const string EndMessage = "End #1";

    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        logger.LogInformation(BeginMessage);
        var response = await next();
        logger.LogInformation(EndMessage);

        return response;
    }
}
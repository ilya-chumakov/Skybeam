using Skybeam.Abstractions;

namespace Skybeam.WebRoot.Handlers;

public class SecondBehavior<TRequest, TResponse>(ILogger<SecondBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
{
    public const string BeginMessage = "Begin #2";
    public const string EndMessage = "End #2";

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
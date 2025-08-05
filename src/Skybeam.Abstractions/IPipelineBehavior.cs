namespace Skybeam.Abstractions;

public interface IPipelineBehavior<in TRequest, TResponse> //where TRequest : notnull
{
    Task<TResponse> HandleAsync(TRequest request, 
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct);
}
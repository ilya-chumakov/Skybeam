using Microsoft.Extensions.DependencyInjection;
using Skybeam.Abstractions;

namespace Skybeam;

// todo to internal
public static class PipelineBuilder
{
    public static RequestHandlerDelegate<TOutput> Decorate<THandler, TInput, TOutput>(
        IServiceProvider provider,
        TInput input,
        CancellationToken ct)
        where THandler : IRequestHandler<TInput, TOutput>
    {
        var handler = provider.GetRequiredService<THandler>();

        var behaviors = provider.GetRequiredService<IEnumerable<IPipelineBehavior<TInput, TOutput>>>().Reverse();

        RequestHandlerDelegate<TOutput> orig = () => handler.HandleAsync(input, ct);
        RequestHandlerDelegate<TOutput> prev = orig;
        
        foreach (var behavior in behaviors)
        {
            RequestHandlerDelegate<TOutput> tmp = prev;
            prev = () => behavior.HandleAsync(input, tmp, ct);
        }
        return prev;
    }
}
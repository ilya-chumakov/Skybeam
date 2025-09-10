using System;
using System.Threading;
using System.Threading.Tasks;
using Skybeam.Abstractions;

namespace Skybeam.Tests.Snapshots.NestedHandler;

public record Alpha;
public record Omega;

public class RootHandler : IRequestHandler<Alpha, Omega>
{
    public RootHandler(IRequestHandler<NestedAlpha, NestedOmega> nestedHandler)
    {
        ArgumentNullException.ThrowIfNull(nestedHandler);
    }

    public Task<Omega> HandleAsync(Alpha input, CancellationToken ct = default)
    {
        
        throw new NotSupportedException();
    }
}

public record NestedAlpha;
public record NestedOmega;

public class NestedHandler : IRequestHandler<NestedAlpha, NestedOmega>
{
    public Task<NestedOmega> HandleAsync(NestedAlpha input, CancellationToken ct = default)
    {
        throw new NotSupportedException();
    }
}

public class LogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public Task<TResponse> HandleAsync(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken ct = default)
    {
        throw new NotSupportedException();
    }
}
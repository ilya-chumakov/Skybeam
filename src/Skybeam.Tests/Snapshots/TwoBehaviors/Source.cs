using System;
using System.Threading;
using System.Threading.Tasks;
using Skybeam.Abstractions;

namespace Skybeam.Tests.Snapshots.TwoBehaviors;

public record Alpha;
public record Omega;

public class BarHandler : IRequestHandler<Alpha, Omega>
{
    public Task<Omega> HandleAsync(Alpha input, CancellationToken ct = default)
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
public class ExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public Task<TResponse> HandleAsync(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken ct = default)
    {
        throw new NotSupportedException();
    }
}
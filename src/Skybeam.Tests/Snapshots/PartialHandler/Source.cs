using System;
using System.Threading;
using System.Threading.Tasks;
using Skybeam.Abstractions;

namespace Skybeam.Tests.Snapshots.PartialHandler;

public record Alpha;
public record Omega;

public partial class BarHandler : IRequestHandler<Alpha, Omega>
{
    public Task<Omega> HandleAsync(Alpha input, CancellationToken ct = default)
    {
        throw new NotSupportedException();
    }
}

public partial class BarHandler : IRequestHandler<Alpha, Omega>
{
    public Task<Omega> Foo()
    {
        throw new NotSupportedException();
    }
}

public partial class BarHandler
{
    public void Bar() { }
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
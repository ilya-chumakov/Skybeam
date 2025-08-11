using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Skybeam.Abstractions;

namespace Skybeam.Tests.Internal;

public class SkybeamFluentBuilder_Tests
{
    // ReSharper disable once InconsistentNaming
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddBehavior_GenericParam_OK()
    {
        //Act
        services.AddSkybeam<DummyRegistry>()
            .AddBehavior(typeof(FirstBehavior<,>))
            .AddBehavior(typeof(SecondBehavior<,>));

        //Assert
        var sp = services.BuildServiceProvider();

        var actual = sp.GetRequiredService<IEnumerable<IPipelineBehavior<Alpha, Omega>>>().ToArray();
        Assert.Equal(2, actual.Length);
        Assert.Equal(typeof(FirstBehavior<Alpha, Omega>), actual[0].GetType());
        Assert.Equal(typeof(SecondBehavior<Alpha, Omega>), actual[1].GetType());
    }

    private class DummyRegistry : IPipelineRegistry
    {
        public void Apply(IServiceCollection services)
        {
        }
    }

    private record Alpha;
    private record Omega;

    private class FirstBehavior<TRequest, TResponse>()
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> HandleAsync(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            var response = await next();
            return response;
        }
    }
    private class SecondBehavior<TRequest, TResponse>()
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> HandleAsync(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            var response = await next();
            return response;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Skybeam.Abstractions;

namespace Skybeam.Tests.Internal;

public class PipelineRegistration_CompositeHandler_Tests
{
    // ReSharper disable once InconsistentNaming
    private readonly ServiceCollection services = new();

    [Fact]
    public void ReplaceWithPipeline_Default_OK()
    {
        //Arrange
        services.AddTransient<IRequestHandler<FooInput, FooOutput>, CompositeHandler>();
        services.AddTransient<IRequestHandler<BarInput, BarOutput>, CompositeHandler>();

        //Act
        services.ReplaceWithPipeline<IRequestHandler<FooInput, FooOutput>, CompositeHandler, FooHandlerPipeline>();
        services.ReplaceWithPipeline<IRequestHandler<BarInput, BarOutput>, CompositeHandler, BarHandlerPipeline>();

        //Assert
        //...Foo
        For<IRequestHandler<FooInput, FooOutput>, CompositeHandler>().Should().BeEmpty();
        For<CompositeHandler, CompositeHandler>().Should().ContainSingle();
        
        For<IRequestHandler<FooInput, FooOutput>, FooHandlerPipeline>().Should().ContainSingle();
        For<FooHandlerPipeline, FooHandlerPipeline>().Should().BeEmpty();
        
        //...Bar
        For<IRequestHandler<BarInput, BarOutput>, CompositeHandler>().Should().BeEmpty();
        For<CompositeHandler, CompositeHandler>().Should().ContainSingle();
        
        For<IRequestHandler<BarInput, BarOutput>, BarHandlerPipeline>().Should().ContainSingle();
        For<BarHandlerPipeline, BarHandlerPipeline>().Should().BeEmpty();

        //...Generic
        services.Where(d => 
                d.ServiceType == typeof(IRequestHandler<,>) || 
                d.ImplementationType == typeof(IRequestHandler<,>))
            .Should().BeEmpty();
    }

    private IEnumerable<ServiceDescriptor> For<TKey, TImpl>()
    {
        return services.Where(d => d.ServiceType == typeof(TKey) && d.ImplementationType == typeof(TImpl));
    }

    private record FooInput;
    private record FooOutput(string Name);

    private record BarInput;
    private record BarOutput(int Id);

    private class CompositeHandler 
        : IRequestHandler<FooInput, FooOutput>
        , IRequestHandler<BarInput, BarOutput>
    {
        public Task<FooOutput> HandleAsync(FooInput input, CancellationToken ct = default)
        {
            return Task.FromResult(new FooOutput("fooName"));
        }

        public Task<BarOutput> HandleAsync(BarInput input, CancellationToken ct = default)
        {
            return Task.FromResult(new BarOutput(42));
        }
    }

    private class FooHandlerPipeline : IRequestHandler<FooInput, FooOutput>
    {
        public Task<FooOutput> HandleAsync(FooInput input, CancellationToken ct = default)
        {
            return Task.FromResult(new FooOutput("fooName"));
        }
    }

    private class BarHandlerPipeline : IRequestHandler<BarInput, BarOutput>
    {
        public Task<BarOutput> HandleAsync(BarInput input, CancellationToken ct = default)
        {
            return Task.FromResult(new BarOutput(42));
        }
    }
}
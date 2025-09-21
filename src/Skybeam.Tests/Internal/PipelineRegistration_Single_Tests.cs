using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skybeam.Abstractions;

namespace Skybeam.Tests.Internal;

public class PipelineRegistration_Single_Tests
{
    // ReSharper disable once InconsistentNaming
    private readonly ServiceCollection services = new();

    [Fact]
    public void ReplaceWithPipeline_Default_OK()
    {
        //Arrange
        services.AddTransient<IRequestHandler<FooInput, FooOutput>, FooHandler>();

        //Act
        services.ReplaceWithPipeline<IRequestHandler<FooInput, FooOutput>, FooHandler, FooHandlerPipeline>();

        //Assert
        AssertCorrectFooHandlerPipeline();
    }

    [Fact]
    public void ReplaceWithPipeline_DuplicatedHandlerRegistration_OK()
    {
        //Arrange
        services.AddTransient<IRequestHandler<FooInput, FooOutput>, FooHandler>();
        services.AddTransient<IRequestHandler<FooInput, FooOutput>, FooHandler>();

        //Act
        services.ReplaceWithPipeline<IRequestHandler<FooInput, FooOutput>, FooHandler, FooHandlerPipeline>();

        //Assert
        AssertCorrectFooHandlerPipeline();
    }

    /// <summary>
    /// Never should happen, defensive case
    /// </summary>
    [Fact]
    public void ReplaceWithPipeline_DuplicatedPipelineRegistration_OK()
    {
        //Arrange
        services.AddTransient<IRequestHandler<FooInput, FooOutput>, FooHandler>();

        //Act
        services.ReplaceWithPipeline<IRequestHandler<FooInput, FooOutput>, FooHandler, FooHandlerPipeline>();
        services.ReplaceWithPipeline<IRequestHandler<FooInput, FooOutput>, FooHandler, FooHandlerPipeline>();

        //Assert
        AssertCorrectFooHandlerPipeline();
    }
    
    [Fact]
    public void ReplaceWithPipeline_AnotherHandlerSameInterface_OK()
    {
        //Arrange
        services.AddTransient<IRequestHandler<FooInput, FooOutput>, FooHandler>();
        services.AddTransient<IRequestHandler<FooInput, FooOutput>, AnotherHandler>();

        //Act
        services.ReplaceWithPipeline<IRequestHandler<FooInput, FooOutput>, FooHandler, FooHandlerPipeline>();

        //Assert
        AssertCorrectFooHandlerPipeline();
        
        // Second handler with the same interface is excluded from registration
        For<IRequestHandler<FooInput, FooOutput>, AnotherHandler>().Should().BeEmpty();
        For<AnotherHandler, AnotherHandler>().Should().BeEmpty();
    }

    private void AssertCorrectFooHandlerPipeline()
    {
        For<IRequestHandler<FooInput, FooOutput>, FooHandler>().Should().BeEmpty();
        For<FooHandler, FooHandler>().Should().ContainSingle();
        
        For<IRequestHandler<FooInput, FooOutput>, FooHandlerPipeline>().Should().ContainSingle();
        For<FooHandlerPipeline, FooHandlerPipeline>().Should().BeEmpty();

        services.Where(d => 
                d.ServiceType == typeof(IRequestHandler<,>) || 
                d.ImplementationType == typeof(IRequestHandler<,>))
            .Should().BeEmpty();
    }

    [Theory]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void ReplaceWithPipeline_CustomLifetime_RespectsLifetime(ServiceLifetime lifetime)
    {
        //Arrange
        services.Add(new ServiceDescriptor(
            typeof(IRequestHandler<FooInput, FooOutput>),
            typeof(FooHandler),
            lifetime));

        //Act
        services.ReplaceWithPipeline<IRequestHandler<FooInput, FooOutput>, FooHandler, FooHandlerPipeline>();

        //Assert
        var descriptor = For<IRequestHandler<FooInput, FooOutput>, FooHandlerPipeline>().Single();

        descriptor.Lifetime.Should().Be(lifetime);
    }

    [Fact]
    public void ReplaceWithPipeline_NoRegistration_AddBothHandlerAndPipeline()
    {
        //Act
        services.ReplaceWithPipeline<IRequestHandler<FooInput, FooOutput>, FooHandler, FooHandlerPipeline>();

        //Assert
        AssertCorrectFooHandlerPipeline();
    }

    private IEnumerable<ServiceDescriptor> For<TKey, TImpl>()
    {
        return services.Where(d => d.ServiceType == typeof(TKey) && d.ImplementationType == typeof(TImpl));
    }

    private record FooInput;

    private record FooOutput(string Name);

    private class FooHandler : IRequestHandler<FooInput, FooOutput>
    {
        public Task<FooOutput> HandleAsync(FooInput input, CancellationToken ct = default)=> Task.FromResult(new FooOutput("fooName"));
    }

    private class FooHandlerPipeline : IRequestHandler<FooInput, FooOutput>
    {
        public Task<FooOutput> HandleAsync(FooInput input, CancellationToken ct = default)=> Task.FromResult(new FooOutput("fooName"));
    }

    /// <summary>
    /// A class implementing the same closed generic handler interface, should be ignored
    /// </summary>
    private class AnotherHandler : IRequestHandler<FooInput, FooOutput>
    {
        public Task<FooOutput> HandleAsync(FooInput input, CancellationToken ct = default)=> Task.FromResult(new FooOutput("fooName"));
    }

}
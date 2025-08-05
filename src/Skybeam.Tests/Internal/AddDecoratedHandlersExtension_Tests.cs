using Microsoft.Extensions.DependencyInjection;
using Skybeam.Abstractions;
using Skybeam;

namespace Skybeam.Tests.Internal;

// hard to isolate due to per-assembly registries
public class AddDecoratedHandlersExtension_AssemblyScan_Tests
{
    // ReSharper disable once InconsistentNaming
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddDecoratedHandlers_AssemblyScan_OK()
    {
        //Arrange
        Assert.False(DummyRegistry.IsInvoked);

        //Act
        services.AddDecoratedHandlers(options =>
        {
            options.ScanAssemblies = [typeof(DummyRegistry).Assembly];
        });
        int expected = services.Count;

        //Assert
        Assert.True(DummyRegistry.IsInvoked);

        //Assert #2: no new registrations
        services.AddDecoratedHandlers(options =>
        {
            options.ScanAssemblies = [typeof(DummyRegistry).Assembly];
        });
        Assert.Equal(expected, services.Count);
    }

    private class DummyRegistry : IPipelineRegistry
    {
        public static bool IsInvoked { get; set; }

        public void Apply(IServiceCollection services)
        {
            IsInvoked = true;
        }
    }
}

public class AddDecoratedHandlersExtension_GenericParam_Tests
{
    // ReSharper disable once InconsistentNaming
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddDecoratedHandlers_GenericParam_OK()
    {
        //Arrange
        Assert.False(DummyRegistry.IsInvoked);

        //Act
        services.AddDecoratedHandlers<DummyRegistry>();
        int expected = services.Count;

        //Assert
        Assert.True(DummyRegistry.IsInvoked);

        //Assert #2 : no new registrations
        services.AddDecoratedHandlers<DummyRegistry>();
        Assert.Equal(expected, services.Count);
    }

    private class DummyRegistry : IPipelineRegistry
    {
        public static bool IsInvoked { get; set; }

        public void Apply(IServiceCollection services)
        {
            IsInvoked = true;
        }
    }
}
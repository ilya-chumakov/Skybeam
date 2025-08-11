using Microsoft.Extensions.DependencyInjection;
using Skybeam.Abstractions;

namespace Skybeam.Tests.Internal;

// hard to isolate due to per-assembly registries
public class AddSkybeam_AssemblyScan_Tests
{
    // ReSharper disable once InconsistentNaming
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddSkybeam_AssemblyScan_OK()
    {
        //Arrange
        Assert.False(DummyRegistry.IsInvoked);

        //Act
        services.AddSkybeam(options =>
        {
            options.ScanAssemblies = [typeof(DummyRegistry).Assembly];
        });
        int expected = services.Count;

        //Assert
        Assert.True(DummyRegistry.IsInvoked);

        //Assert #2: no new registrations
        services.AddSkybeam(options =>
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

public class AddSkybeam_GenericParam_Tests
{
    // ReSharper disable once InconsistentNaming
    private readonly ServiceCollection services = new();

    [Fact]
    public void AddSkybeam_GenericParam_OK()
    {
        //Arrange
        Assert.False(DummyRegistry.IsInvoked);

        //Act
        services.AddSkybeam<DummyRegistry>();
        int expected = services.Count;
        
        //Assert
        Assert.True(DummyRegistry.IsInvoked);

        //Assert #2 : no new registrations
        services.AddSkybeam<DummyRegistry>();
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
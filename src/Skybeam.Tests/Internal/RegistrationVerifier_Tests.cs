using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Skybeam.Abstractions;
using Skybeam;

namespace Skybeam.Tests.Internal;

public class RegistrationVerifier_Tests
{
    private readonly ServiceCollection _services = new();
    private readonly DelayedLog _log = new();

    [Theory]
    [InlineData(0,1)]
    [InlineData(1,0)]
    [InlineData(2,1)]
    public void VerifyRegistryCount_ChecksTypeCount_ProducesLogsIfIncorrect(int typeCount, int expectedLogCount)
    {
        //Arrange
        RegistrationVerifier verifier = new(_log);
        var registryTypes = Enumerable.Repeat(typeof(Foo), typeCount).ToArray();

        //Act
        verifier.VerifyRegistryCount(registryTypes);

        //Assert
        Assert.Equal(expectedLogCount, _log.Count());
    }

    [Fact]
    public void VerifyServices_NoRegistrations_ProducesLogs()
    {
        //Arrange
        RegistrationVerifier verifier = new(_log);

        //Act
        verifier.VerifyServices(_services);

        //Assert
        Assert.Equal(1, _log.Count());
    }

    [Fact]
    public void VerifyServices_HandlerRegistered_NoLogs()
    {
        //Arrange
        _services.AddTransient<IRequestHandler<Foo, Bar>, FooBarHandler>();
        RegistrationVerifier verifier = new(_log);

        //Act
        verifier.VerifyServices(_services);

        //Assert
        Assert.Equal(0, _log.Count());
    }

    [Fact]
    public void VerifyServices_HandlerWithTheSameInput_ProducesLogs()
    {
        //Arrange
        _services.AddTransient<IRequestHandler<Foo, Bar>, FooBarHandler>();
        _services.AddTransient<IRequestHandler<Foo, Gamma>, DuplicateInputHandler>();
        RegistrationVerifier verifier = new(_log);

        //Act
        verifier.VerifyServices(_services);

        //Assert
        Assert.Equal(1, _log.Count());
    }

    private record Foo;
    private record Bar;
    private record Gamma;

    private class FooBarHandler : IRequestHandler<Foo, Bar>
    {
        public Task<Bar> HandleAsync(Foo input, CancellationToken ct = default)
        {
            return Task.FromResult(new Bar());
        }
    }

    private class DuplicateInputHandler : IRequestHandler<Foo, Gamma>
    {
        public Task<Gamma> HandleAsync(Foo input, CancellationToken ct = default)
        {
            return Task.FromResult(new Gamma());
        }
    }
}
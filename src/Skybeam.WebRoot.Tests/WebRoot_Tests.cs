using System.Reflection;
using Skybeam.Abstractions;
using FancyGlobalPrefix;
using FancyGlobalPrefix.Skybeam.WebRoot.Handlers;
using FluentAssertions;
using Meziantou.Extensions.Logging.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skybeam.WebRoot.Handlers;

// ReSharper disable InconsistentNaming

namespace Skybeam.WebRoot.Tests;

public class WebRoot_Tests
{
    private readonly InMemoryLoggerProvider loggerProvider = new();
    private readonly ServiceCollection services = new();

    public WebRoot_Tests(ITestOutputHelper output)
    {
        services.AddLogging();
        services.AddSingleton<ILoggerProvider>(loggerProvider);
    }

    [Fact]
    public async Task WebRoot_UseGeneratedCode_PipelineIsResolvedAndCorrect()
    {
        services.AddTransient<IRequestHandler<FooQuery, FooResponse>, FooQueryHandler>();
        
        services.AddSkybeam<PipelineRegistry>()
            .AddBehavior(typeof(FirstBehavior<,>))
            .AddBehavior(typeof(SecondBehavior<,>))
            ;

        var provider = services.BuildServiceProvider();

        // Act
        var actual = provider.GetRequiredService<IRequestHandler<FooQuery, FooResponse>>();

        // Assert: type is changed
        actual.GetType().Should().NotBe(typeof(FooQueryHandler));
        actual.GetType().Should().Be(typeof(FooQueryHandlerPipeline));

        // Assert: decorators are called
        FooResponse output = await actual.HandleAsync(new FooQuery(), CancellationToken.None);

        loggerProvider.Logs.Informations.Select(l => l.Message)
            .Should().BeEquivalentTo(
                FirstBehavior<FooQuery, FooResponse>.BeginMessage,
                SecondBehavior<FooQuery, FooResponse>.BeginMessage,
                FooQueryHandler.Message,
                SecondBehavior<FooQuery, FooResponse>.EndMessage,
                FirstBehavior<FooQuery, FooResponse>.EndMessage);
    }

    [Fact]
    public void Wrapper_AvailableViaReflection_OK()
    {
        var type = Assembly.GetAssembly(typeof(FooQueryHandler))!
            .GetType("FancyGlobalPrefix.Skybeam.WebRoot.Handlers.FooQueryHandlerPipeline", true, true);

        type.Should().NotBeNull();
    }

    [Fact]
    public void Wrapper_AvailableInCompileTime_OK()
    {
        var x = new FooQueryHandlerPipeline(null);
    }
}
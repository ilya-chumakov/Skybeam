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

public class GenerationTests
{
    private readonly InMemoryLoggerProvider loggerProvider = new();
    private readonly ServiceCollection services = new();

    public GenerationTests(ITestOutputHelper output)
    {
        services.AddLogging();
        services.AddSingleton<ILoggerProvider>(loggerProvider);
    }

    [Fact]
    public async Task AddPipelines_WrapperIsGenerated_HandlerIsReplaced()
    {
        services.AddTransient<IRequestHandler<FooQuery, FooResponse>, FooQueryHandler>();
        
        //todo register by interface, resolve as IEnumerable<T>
        services.AddTransient<
            //IPipelineBehavior<FooQuery, FooResponse>,
            FooFirstBehavior<FooQuery, FooResponse>>();

        services.AddTransient<
            //IPipelineBehavior<FooQuery, FooResponse>,
            FooSecondBehavior<FooQuery, FooResponse>>();

        services.AddSkybeam<PipelineRegistry>();

        var provider = services.BuildServiceProvider();

        // Act
        var actual = provider.GetRequiredService<IRequestHandler<FooQuery, FooResponse>>();

        // Assert: type is changed
        actual.GetType().Should().NotBe(typeof(FooQueryHandler));
        actual.GetType().Should().Be(typeof(FooQueryHandlerPipeline));

        // Assert: decorators are called
        await actual.HandleAsync(new FooQuery(), CancellationToken.None);

        var logs = loggerProvider.Logs.Informations.ToList();
        logs[0].Message.Should().BeEquivalentTo($"Hello from the behavior #1. Decorating {nameof(FooQuery)}.");
        logs[0].Message.Should().BeEquivalentTo($"Hello from the behavior #2. Decorating {nameof(FooQuery)}.");
        logs[2].Message.Should().BeEquivalentTo("FooQueryHandler is called!");
        logs[3].Message.Should().BeEquivalentTo("Bye from the behavior #2");
        logs[4].Message.Should().BeEquivalentTo("Bye from the behavior #1");
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
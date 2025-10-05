using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Meziantou.Extensions.Logging.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skybeam.Abstractions;
using ILoggerProvider = Microsoft.Extensions.Logging.ILoggerProvider;

namespace Skybeam.Tests.ServiceRegistration;

public class PipelineBuilder_Tests
{
    [Fact]
    public async Task Decorate_TwoBehaviors_RunCorrectOrder()
    {
        var loggerProvider = new InMemoryLoggerProvider();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<ILoggerProvider>(loggerProvider);

        services.AddTransient<FooHandler>();

        services.AddTransient<
            IPipelineBehavior<Alpha, Omega>,
            FirstBehavior<Alpha, Omega>>();

        services.AddTransient<
            IPipelineBehavior<Alpha, Omega>,
            SecondBehavior<Alpha, Omega>>();

        var sp = services.BuildServiceProvider();

        RequestHandlerDelegate<Omega> target = PipelineBuilder.Decorate<FooHandler, Alpha, Omega>(
            sp, new Alpha(), TestContext.Current.CancellationToken);

        // Act
        Omega output = await target.Invoke();

        loggerProvider.Logs.Informations.Select(l => l.Message)
            .Should().BeEquivalentTo(
                FirstBehavior<Alpha, Omega>.BeginMessage,
                SecondBehavior<Alpha, Omega>.BeginMessage,
                FooHandler.Message,
                SecondBehavior<Alpha, Omega>.EndMessage,
                FirstBehavior<Alpha, Omega>.EndMessage);
    }

    [Fact]
    public async Task Decorate_NoBehaviors_RunOnlyHandler()
    {
        var loggerProvider = new InMemoryLoggerProvider();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<ILoggerProvider>(loggerProvider);

        services.AddTransient<FooHandler>();

        var sp = services.BuildServiceProvider();

        RequestHandlerDelegate<Omega> target = PipelineBuilder.Decorate<FooHandler, Alpha, Omega>(
            sp, new Alpha(), TestContext.Current.CancellationToken);

        // Act
        Omega output = await target.Invoke();

        loggerProvider.Logs.Informations.Select(l => l.Message)
            .Should().BeEquivalentTo(FooHandler.Message);
    }
}

file record Alpha;

file record Omega;

file class FooHandler(ILogger<FooHandler> logger) : IRequestHandler<Alpha, Omega>
{
    public static readonly string Message = nameof(FooHandler) + " is called.";

    public Task<Omega> HandleAsync(Alpha input, CancellationToken ct = default)
    {
        logger.LogInformation(Message);
        return Task.FromResult(new Omega());
    }
}

file class FirstBehavior<TRequest, TResponse>(ILogger<FirstBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
{
    public const string BeginMessage = "Begin #1";
    public const string EndMessage = "End #1";

    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        logger.LogInformation(BeginMessage);
        var response = await next();
        logger.LogInformation(EndMessage);

        return response;
    }
}

file class SecondBehavior<TRequest, TResponse>(ILogger<SecondBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
{
    public const string BeginMessage = "Begin #2";
    public const string EndMessage = "End #2";

    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        logger.LogInformation(BeginMessage);
        var response = await next();
        logger.LogInformation(EndMessage);

        return response;
    }
}
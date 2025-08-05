using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Meziantou.Extensions.Logging.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skybeam;

// ReSharper disable InconsistentNaming

namespace Skybeam.Tests;

public class GenDemo
{
    private readonly InMemoryLoggerProvider loggerProvider = new();
    private readonly ServiceCollection services = new();

    public GenDemo(ITestOutputHelper output)
    {
        services.AddSingleton<ILoggerProvider>(loggerProvider);
        //services.AddLogging(cfg =>
        //{
        //    cfg.AddXunit(output);
        //});
    }

    [Fact(Skip = "under construction")]
    public async Task ConcreteHandler_WrapperIsGenerated_HandlerIsReplaced()
    {
        //services.AddTransient<IConcreteHandler, ConcreteHandler>();
        //services.AddTransient<FirstDecorator>();
        //services.AddTransient<SecondDecorator>();

        // before this line everything is registered in a natural way.
        // now replace our handler registration with a source-generated wrapper.
        services.AddPipelines();
        var provider = services.BuildServiceProvider();

        // Act
        //var actual = provider.GetRequiredService<IConcreteHandler>();

        // type is changed
        //actual.GetType().Should().NotBe(typeof(ConcreteHandler));

        // decorators are called
        //await actual.HandleAsync();

        var logs = loggerProvider.Logs.Informations.ToList();
        logs[0].Message.Equals("Hello from the decorator #2").Should().BeTrue();
        logs[1].Message.Equals("Hello from the decorator #1").Should().BeTrue();
        logs[2].Message.Equals("ConcreteHandler is called!").Should().BeTrue();
        logs[3].Message.Equals("Bye from the decorator #1").Should().BeTrue();
        logs[4].Message.Equals("Bye from the decorator #2").Should().BeTrue();
    }
}
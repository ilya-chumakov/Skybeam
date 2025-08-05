//using Bodrocode.Xunit.Logs;

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

//using Xunit.Abstractions;

namespace Skybeam.Playground;

public class RequestHandlerTests(ITestOutputHelper output)
{
    [Fact]
    public async Task ConcreteHandler_WrapperIsCoded_HandlerIsReplaced()
    {
        var services = new ServiceCollection();
        //services.AddLogging(cfg =>
        //{
        //    cfg.AddXunit(output);
        //});
        services.AddLogging();
        services.AddTransient<IRequestHandler<BarQuery, BarResponse>, BarQueryHandler>();
        services.AddTransient(typeof(FirstBehavior<,>));
        services.AddTransient(typeof(SecondBehavior<,>));

        // Before this line everything is registered in a natural way.
        // Now replace our handler registration with a source-generated wrapper.
        services.ReplaceHandlerWithPipeline();
        var provider = services.BuildServiceProvider();

        var actual = provider.GetRequiredService<IRequestHandler<BarQuery, BarResponse>>();

        await actual.HandleAsync(new BarQuery());

        actual.GetType().Should().Be(typeof(BarQueryHandlerPipeline));
    }
}
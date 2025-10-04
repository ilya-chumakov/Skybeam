using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Skybeam;

internal class DelayedLogHostedService(ILogger<DelayedLogHostedService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        AddSkybeamExtension.Log.Replay(logger);

        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
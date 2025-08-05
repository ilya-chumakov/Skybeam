using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Skybeam;

// InternalsVisibleTo won't work if directly called from another assembly
public static class RegistrationExtensions
{
    public static void ReplaceWithPipeline<TClosedInterface, THandler, TPipeline>(
        this IServiceCollection services)
        where THandler : class, TClosedInterface
        where TPipeline : class, TClosedInterface
    {
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TClosedInterface));

        // no messing with auto-registations for now
        if (descriptor == null) return;

        var lifetime = descriptor?.Lifetime ?? ServiceLifetime.Transient;

        services.RemoveAll(typeof(TClosedInterface));

        services.TryAdd(new ServiceDescriptor(
            typeof(TClosedInterface),
            typeof(TPipeline),
            lifetime));

        services.TryAdd(new ServiceDescriptor(
            typeof(THandler),
            typeof(THandler),
            lifetime));
    }
}
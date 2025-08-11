using Microsoft.Extensions.DependencyInjection;
using Skybeam.Abstractions;

namespace Skybeam;

public class SkybeamFluentBuilder(IServiceCollection services)
{
    public SkybeamFluentBuilder AddBehavior(Type behaviorType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        if (!behaviorType.IsGenericType)
        {
            throw new InvalidOperationException($"{behaviorType.Name} must be generic");
        }

        var interfaces = behaviorType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Select(i => i.GetGenericTypeDefinition())
                .Where(i => i == typeof(IPipelineBehavior<,>))
                .ToHashSet();

        if (interfaces.Count == 0)
        {
            throw new InvalidOperationException($"{behaviorType.Name} must implement {typeof(IPipelineBehavior<,>).FullName}");
        }

        foreach (var bi in interfaces)
        {
            services.Add(new ServiceDescriptor(bi, behaviorType, serviceLifetime));
        }
        return this;
    }
}
public static class EnumerableExtensions
{
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => [..source];
}
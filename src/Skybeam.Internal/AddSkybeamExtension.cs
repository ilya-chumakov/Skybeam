using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Skybeam.Abstractions;

namespace Skybeam;

public class SkybeamOptions
{
    public IReadOnlyCollection<Assembly> ScanAssemblies = null;
}

// InternalsVisibleTo won't work if directly called from another assembly
public static class AddSkybeamExtension
{
    private static readonly Type RegistryInterface = typeof(IPipelineRegistry);
    private static readonly RegistrationVerifier Verifier;
    internal static readonly DelayedLog Log = new();

    static AddSkybeamExtension()
    {
        Verifier = new RegistrationVerifier(Log);
    }

    public static void AddSkybeam<TPipelineRegistry>(
        this IServiceCollection services)
        where TPipelineRegistry : IPipelineRegistry, new()
    {
        IPipelineRegistry registry = new TPipelineRegistry();
        registry.Apply(services);
        Verifier.VerifyServices(services);
        Verifier.VerifyRegistry(registry);
        services.AddHostedService<DelayedLogHostedService>();
    }

    public static void AddSkybeam(
        this IServiceCollection services,
        Action<SkybeamOptions> setup = null)
    {
        var options = new SkybeamOptions();
        setup?.Invoke(options);

        IEnumerable<Assembly> assemblies = options.ScanAssemblies;

        if (options.ScanAssemblies is not { Count: > 0 })
        {
            // todo any better ideas for speeding up this assembly scan?
            assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Where(x => !x.FullName.StartsWith("Microsoft"))
                .Where(x => !x.FullName.StartsWith("System"))
                .Where(x => !x.FullName.StartsWith("netstandard"))
                .Where(x => !x.FullName.StartsWith("JetBrains"))
                .Where(x => !x.FullName.StartsWith("xunit"));
        }

        object[] parameters = [services];
        List<Type> foundTypes = new();

        foreach (Assembly assembly in assemblies)
        {
            IEnumerable<Type> types = assembly.GetTypes().Where(type =>
                RegistryInterface.IsAssignableFrom(type) && type != RegistryInterface);

            foreach (Type registryType in types)
            {
                var registry = InvokeApplyMethod(registryType, parameters);
                foundTypes.Add(registryType);
                Verifier.VerifyRegistry(registry);
            }
        }

        Verifier.VerifyRegistryCount(foundTypes);
        Verifier.VerifyServices(services);
        services.AddHostedService<DelayedLogHostedService>();
    }

    private static IPipelineRegistry InvokeApplyMethod(Type registryType, object[] parameters)
    {
        if (registryType == null) return null;

        var method = registryType.GetMethod(nameof(IPipelineRegistry.Apply));

        if (method == null) return null;

        object registry = Activator.CreateInstance(registryType);

        method.Invoke(registry, parameters);

        return registry as IPipelineRegistry;
    }
}
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Skybeam.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Skybeam;

public static class AddSkybeamExtension
{
    private static readonly Type RegistryInterface = typeof(IPipelineRegistry);
    private static readonly RegistrationVerifier Verifier;
    internal static readonly DelayedLog Log = new();

    static AddSkybeamExtension()
    {
        Verifier = new RegistrationVerifier(Log);
    }

    /// <summary>
    /// Add source-generated pipelines to the services list.
    /// <br/>
    /// As a part of recommended setup, reference Skybeam package once per app and call <see cref="AddSkybeam&lt;TPipelineRegistry&gt;"/>
    /// providing a single generated pipeline registry as a parameter.
    /// <br/>
    /// Prior handler registration is not needed. Existing handler registrations are replaced with their respectful pipeline registrations as well.
    /// <br/>
    /// </summary>
    /// <typeparam name="TPipelineRegistry">Source-generated pipeline registry</typeparam>
    /// <param name="services"></param>
    /// <remarks>
    /// This method is safe to call multiple times.
    /// </remarks>
    public static SkybeamFluentBuilder AddSkybeam<TPipelineRegistry>(
        this IServiceCollection services)
        where TPipelineRegistry : IPipelineRegistry, new()
    {
        IPipelineRegistry registry = new TPipelineRegistry();
        registry.Apply(services);
        Verifier.VerifyServiceRegistrations(services);
        Verifier.VerifyRegistry(registry);
        services.AddHostedService<DelayedLogHostedService>();

        return new SkybeamFluentBuilder(services);
    }

    /// <summary>
    /// Add source-generated pipelines to the services list. This method is an optional fallback to a reflection-based assembly scan.
    /// <br/>
    /// As a part of recommended setup, reference Skybeam package once per app and call <see cref="AddSkybeam&lt;TPipelineRegistry&gt;"/>
    /// providing a single generated pipeline registry as a parameter.
    /// <br/>
    /// Prior handler registration is not needed. Existing handler registrations are replaced with their respectful pipeline registrations as well.
    /// <br/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setup"></param>
    /// <remarks>
    /// This method is safe to call multiple times.
    /// This method is NOT AOT-friendly.
    /// </remarks>
    public static SkybeamFluentBuilder AddSkybeam(
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
        List<Type> foundRegistryTypes = new();
        Func<Type, bool> filter = t => options.RegistryFilter == null || options.RegistryFilter.Invoke(t);

        foreach (Assembly assembly in assemblies)
        {
            IEnumerable<Type> types = assembly.GetTypes().Where(type =>
                RegistryInterface.IsAssignableFrom(type) && type != RegistryInterface);

            foreach (Type registryType in types.Where(filter))
            {
                var registry = InvokeApplyMethod(registryType, parameters);
                foundRegistryTypes.Add(registryType);
                Verifier.VerifyRegistry(registry);
            }
        }

        Verifier.VerifyRegistryCount(foundRegistryTypes);
        Verifier.VerifyServiceRegistrations(services);
        services.AddHostedService<DelayedLogHostedService>();
        
        return new SkybeamFluentBuilder(services);
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
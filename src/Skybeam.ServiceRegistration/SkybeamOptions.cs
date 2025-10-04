using System.Reflection;

namespace Skybeam;

public class SkybeamOptions
{
    /// <summary>
    /// Setup the only assemblies to look for a pipeline registries.
    /// </summary>
    public IReadOnlyCollection<Assembly> ScanAssemblies = null;

    /// <summary>
    /// Useful for test isolation after an app-wide assembly scan. Not a part of a public API. 
    /// </summary>
    internal Func<Type, bool> RegistryFilter = null;
}
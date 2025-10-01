using Skybeam.Tests.Models;

namespace Skybeam.Tests.Snapshots.NoBehaviors;

internal class SourceDescription : SourceDescriptionBase
{
    public SourceDescription()
    {
        Handlers.Add(new(
            Name: nameof(BarHandler),
            FullName: GetDisplayFullName<BarHandler>(),
            ContainingNamespace: typeof(BarHandler).Namespace,
            InputFullName: nameof(Alpha),
            OutputFullName: nameof(Omega)));
            
        SourceFiles.Add(DefaultSourceFile);
    }
}
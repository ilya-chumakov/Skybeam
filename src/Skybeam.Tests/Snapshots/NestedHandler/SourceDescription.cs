using Skybeam;
using Skybeam.Tests.Models;

namespace Skybeam.Tests.Snapshots.NestedHandler;

internal class SourceDescription : SourceDescriptionBase
{
    public SourceDescription()
    {
        Handlers.Add(new(
            Name: nameof(RootHandler),
            FullName: GetDisplayFullName<RootHandler>(),
            ContainingNamespace: typeof(RootHandler).Namespace,
            InputFullName: GetDisplayFullName<Alpha>(),
            OutputFullName: GetDisplayFullName<Omega>()));

        Handlers.Add(new(
            Name: nameof(NestedHandler),
            FullName: GetDisplayFullName<NestedHandler>(),
            ContainingNamespace: typeof(NestedHandler).Namespace,
            InputFullName: GetDisplayFullName<NestedAlpha>(),
            OutputFullName: GetDisplayFullName<NestedOmega>()));

        Behaviors.Add(new BehaviorDescription(
                GetDisplayFullName<LogBehavior<string, string>>()
            )
        );

        SourceFiles.Add(DefaultSourceFile);
        ExpectedFiles.Add(new("ExpectedRootPipeline.cs", "RootHandler_Pipeline.g.cs"));
        ExpectedFiles.Add(new("ExpectedNestedPipeline.cs", "NestedHandler_Pipeline.g.cs"));
        ExpectedFiles.Add(DefaultExpectedRegistry);
    }
}
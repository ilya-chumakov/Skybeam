using Skybeam;
using Skybeam.Tests.Models;

namespace Skybeam.Tests.Snapshots.SameName;

public class SourceDescription : SourceDescriptionBase
{
    public SourceDescription()
    {
        Handlers.Add(new(
            Name: nameof(HandlerNamespace.Foo),
            FullName: GetDisplayFullName<HandlerNamespace.Foo>(),
            ContainingNamespace: typeof(HandlerNamespace.Foo).Namespace,
            InputFullName: GetDisplayFullName<RequestNamespace.Foo>(),
            OutputFullName: GetDisplayFullName<ResponseNamespace.Foo>()));

        Behaviors.Add(new BehaviorDescription(
                GetDisplayFullName<BehaviorNamespace.Foo<string, string>>()
            )
        );

        SourceFiles.Add(DefaultSourceFile);
        ExpectedFiles.Add(new FileDescription("ExpectedPipeline.cs", "Foo_Pipeline.g.cs"));
        ExpectedFiles.Add(DefaultExpectedRegistry);
    }
}
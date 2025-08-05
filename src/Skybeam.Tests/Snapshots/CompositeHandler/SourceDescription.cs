using Skybeam;
using Skybeam.Tests.Models;

namespace Skybeam.Tests.Snapshots.CompositeHandler;

public class SourceDescription : SourceDescriptionBase
{
    public SourceDescription()
    {
        Handlers.Add(new(
            Name: nameof(BarHandler),
            FullName: GetDisplayFullName<BarHandler>(),
            ContainingNamespace: typeof(BarHandler).Namespace,
            InputFullName: GetDisplayFullName<Alpha>(),
            OutputFullName: GetDisplayFullName<Omega>())
        );
        
        Handlers.Add(new(
            Name: nameof(BarHandler),
            FullName: GetDisplayFullName<BarHandler>(),
            ContainingNamespace: typeof(BarHandler).Namespace,
            InputFullName: GetDisplayFullName<Beta>(),
            OutputFullName: GetDisplayFullName<Omega>(),
            PipelineSuffix: "_1")
        );
        
        Behaviors.Add(new BehaviorDescription(
                GetDisplayFullName<LogBehavior<string, string>>()
            )
        );

        SourceFiles.Add(DefaultSourceFile);
        ExpectedFiles.Add(new FileDescription("ExpectedPipelineAlpha.cs", "BarHandler_Pipeline.g.cs"));
        ExpectedFiles.Add(new FileDescription("ExpectedPipelineBeta.cs", "BarHandler_Pipeline_1.g.cs"));

        ExpectedFiles.Add(DefaultExpectedRegistry);
    }
}
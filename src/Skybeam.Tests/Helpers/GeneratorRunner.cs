using System.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Skybeam.Abstractions;

namespace Skybeam.Tests.Helpers;

// todo remove?
public static class GeneratorRunner
{
    public static GeneratorDriverRunResult GetRunResult(string source)
    {
        var generator = new PipelineGenerator();
        var compilation = CSharpCompilation.Create("TestAssembly",
            syntaxTrees: [CSharpSyntaxTree.ParseText(source)],
            references:
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(AssemblyTargetedPatchBandAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IRequestHandler<,>).Assembly.Location)
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        return CSharpGeneratorDriver.Create(generator).RunGenerators(compilation).GetRunResult();
    }
}
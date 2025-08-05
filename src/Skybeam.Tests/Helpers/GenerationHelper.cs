using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using Skybeam.Tests.Models;

namespace Skybeam.Tests.Helpers;

public static class GenerationHelper
{
    public static async Task AssertGenerationEquality(
        List<TestFile> sourceFiles, 
        List<TestFile> expectedFiles)
    {
        var test = new Verifier.Test();
        foreach (TestFile file in sourceFiles)
        {
            test.TestState.Sources.Add((filename: "SomeUnusedName.cs", content: file.Content));
        }

        foreach (TestFile result in expectedFiles)
        {
            test.TestState.GeneratedSources.Add((
                sourceGeneratorType: typeof(PipelineGenerator),
                filename: result.Name,
                content: SourceText.From(result.Content, Encoding.UTF8)
            ));
        }
        await test.RunAsync();
    }
}
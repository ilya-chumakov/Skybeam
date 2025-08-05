using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using Skybeam.Tests.Helpers;
using Skybeam.Tests.Models;

//using Xunit.Abstractions;

namespace Skybeam.Tests;

public class PipelineGeneratorTests(ITestOutputHelper output)
{
    [Fact]
    public async Task GeneratorOutput_NoBehavior_OK()
    {
        await VerifyGenerationFrom<Snapshots.NoBehaviors.SourceDescription>();
    }

    [Fact]
    public async Task GeneratorOutput_OneBehavior_OK()
    {
        await VerifyGenerationFrom<Snapshots.OneBehavior.SourceDescription>();
    }

    [Fact]
    public async Task GeneratorOutput_TwoBehaviors_OK()
    {
        await VerifyGenerationFrom<Snapshots.TwoBehaviors.SourceDescription>();
    }

    [Fact]
    public async Task GeneratorOutput_CompositeHandler_OK()
    {
        await VerifyGenerationFrom<Snapshots.CompositeHandler.SourceDescription>();
    }

    [Fact]
    public async Task GeneratorOutput_SameName_OK()
    {
        await VerifyGenerationFrom<Snapshots.SameName.SourceDescription>();
    }

    [Fact]
    public async Task GeneratorOutput_PartialHandler_OK()
    {
        await VerifyGenerationFrom<Snapshots.PartialHandler.SourceDescription>();
    }

    [Fact]
    public async Task GeneratorOutput_PartialBehavior_OK()
    {
        await VerifyGenerationFrom<Snapshots.PartialBehavior.SourceDescription>();
    }

    private async Task VerifyGenerationFrom<TSourceDescription>()
        where TSourceDescription : SourceDescriptionBase, new()
    {
        SourceDescriptionBase description = new TSourceDescription();

        var sourceFiles = await SnapshotReader.ReadAsync(description.FolderName, description.SourceFiles);
        var expectedFiles = await SnapshotReader.ReadAsync(description.FolderName, description.ExpectedFiles);

        // compilation
        // optional, files are included in the project now, therefore step is no needed
        if (false)
        {
            foreach (var file in sourceFiles)
            {
                CompilationHelper.AssertCompilation(file.Content);
            }
        }

        // text
        if (expectedFiles.Count > 0)
        {
            var pipelines = new List<PipelineDescription>(description.Handlers.Count);

            for (int i = 0; i < description.Handlers.Count; i++)
            {
                (SourceText text, var pipeline) = PipelineTextEmitter.CreateSourceText(
                    description.Handlers[i], description.Behaviors);

                string actual = text.ToString();

                AssertEquality(expectedFiles[i], actual);

                pipelines.Add(pipeline);
            }

            string actualContext = RegistryTextEmitter.CreateSourceText(pipelines).ToString();

            AssertEquality(expectedFiles[^1], actualContext);
        }

        // generation
        await GenerationHelper.AssertGenerationEquality(sourceFiles, expectedFiles);
    }

    private void AssertEquality(TestFile expectedFile, string actual)
    {
        output.WriteLine("Asserting with the expected file: " + expectedFile.Name);
        output.WriteLine("");

        TextHelper.AssertEqualityWithDiffPlex(expectedFile.Content, actual, output);
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using Skybeam.Tests.Helpers;
using Skybeam.Tests.Helpers.Legacy;
using Skybeam.Tests.Models;

namespace Skybeam.Tests;

public class PipelineGenerator_Tests(ITestOutputHelper output)
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

    [Fact]
    public async Task GeneratorOutput_NestedHandler_OK()
    {
        await VerifyGenerationFrom<Snapshots.NestedHandler.SourceDescription>();
    }

    private async Task VerifyGenerationFrom<TSourceDescription>()
        where TSourceDescription : SourceDescriptionBase, new()
    {
        SourceDescriptionBase description = new TSourceDescription();

        var sourceFiles = await SnapshotReader.ReadAsync(description.FolderName, description.SourceFiles);
        var expectedFiles = await SnapshotReader.ReadAsync(description.FolderName, description.ExpectedFiles);

        // compilation
        // optional, files are included in the project now, therefore step is no needed
#pragma warning disable CS0162 // Unreachable code detected
        if (false)
        {
            foreach (var file in sourceFiles)
            {
                LegacyCompilationHelper.AssertCompilation(file.Content);
            }
        }
#pragma warning restore CS0162 // Unreachable code detected

        // text
        var textResults = new List<ComparisonResult>();
        if (expectedFiles.Count > 0)
        {
            var pipelines = new List<PipelineDescription>(description.Handlers.Count);

            for (int i = 0; i < description.Handlers.Count; i++)
            {
                (SourceText text, var pipeline) = PipelineTextEmitter.CreateSourceText(
                    description.Handlers[i], description.Behaviors);

                string actual = text.ToString();

                var pipelineResult = Compare(expectedFiles[i], actual);

                pipelines.Add(pipeline);
                textResults.Add(pipelineResult);
            }

            string actualContext = RegistryTextEmitter.CreateSourceText(pipelines, "TestProject").ToString();
            //string actualContext = RegistryTextEmitter.CreateSourceText(pipelines, description.FolderName).ToString();

            var registryResult = Compare(expectedFiles[^1], actualContext);
            textResults.Add(registryResult);
        }

        foreach (var r in textResults)
        {
            string status = r.IsSuccess ? "OK" : "DIFF";
            output.WriteLine($"{r.ExpectedFileName}: {status}");
        }

        var errors = textResults.Where(x => !x.IsSuccess).ToList();
        
        for (int i = 0; i < errors.Count; i++)
        {
            var r = errors[i];
            output.WriteLine("");
            output.WriteLine($"/////// Diff details #{i} begin: {r.ExpectedFileName}");
            output.WriteLine("/////// Diff: ");
            output.WriteLine(r.Diff);
            output.WriteLine("");
            output.WriteLine("/////// Expected: ");
            output.WriteLine(r.ExpectedContent);
            output.WriteLine("");
            output.WriteLine("/////// Actual:");
            output.WriteLine(r.ActualContent);
            output.WriteLine($"/////// Diff details #{i} end: {r.ExpectedFileName}");
        }

        if (errors.Count > 0) Assert.Fail("Emitted text differs from a snapshot!");

        // generation
        await SourceGenerationTestRunner.AssertGenerationEquality(sourceFiles, expectedFiles, description.FolderName);
    }

    private ComparisonResult Compare(TestFile expectedFile, string actual)
    {
        string diff = TextHelper.GetUnidiff(expectedFile.Content, actual);

        return new ComparisonResult
        {
            ExpectedFileName = expectedFile.Name,
            ExpectedContent = expectedFile.Content,
            ActualContent = actual,
            Diff = diff
        };
    }
}

public readonly record struct ComparisonResult(
    string ExpectedFileName,
    string ExpectedContent,
    string ActualContent,
    string Diff)
{
    public bool IsSuccess => string.IsNullOrWhiteSpace(Diff);
};
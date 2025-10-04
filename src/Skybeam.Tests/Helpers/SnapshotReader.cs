using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Skybeam.Tests.Models;

namespace Skybeam.Tests.Helpers;

public static class SnapshotReader
{
    private static readonly string GeneratorAssemblyVersion =
        typeof(PipelineTextEmitter).Assembly.GetName().Version?.ToString();

    public static async Task<List<TestFile>> ReadAsync(
        string folderName,
        List<FileDescription> files)
    {
        List<TestFile> sources = new();
        foreach (var description in files)
        {
            string content = await ReadAllTextAsync(folderName, description.RelativePath);

            sources.Add(new TestFile
            {
                Name = description.GeneratedFileName,
                Content = content
            });
        }
        return sources;
    }

    public static async Task<string> ReadAllTextAsync(
        string folderName, string relativePath)
    {
        string path = Path.Combine("Snapshots", folderName, relativePath);
        string content = await File.ReadAllTextAsync(path);

        string normalized = LineEndingsHelper.Normalize(content)
            .Replace("%VERSION%", GeneratorAssemblyVersion)
            .Replace(
                $"namespace SnapshotNamespacePlaceholder.{folderName};", 
                //$"namespace {PipelineTextEmitter.NamespacePrefix}.{folderName};"

                //had to use "TestProject" due to hard-coded DefaultTestProjectName in CSharpSourceGeneratorTest
                $"namespace {PipelineTextEmitter.NamespacePrefix}.TestProject;"
                )
            ;

        return normalized;
    }
}
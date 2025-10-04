using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Skybeam.Tests.Models;

namespace Skybeam.Tests.Helpers;

public static class SnapshotReader
{
    private static readonly string GeneratorAssemblyVersion =
        typeof(PipelineEmitter).Assembly.GetName().Version?.ToString();

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
            
            // Setting up namespaces for the generated code.
            // Registry namespace:
            .Replace(
                //had to use "TestProject" due to hard-coded DefaultTestProjectName in CSharpSourceGeneratorTest
                $"namespace RegistryNamespacePlaceholder.{folderName};", 
                $"namespace {PipelineEmitter.NamespacePrefix}.TestProject;"
                //$"namespace {PipelineEmitter.NamespacePrefix}.{folderName};"
            )
            
            // Pipeline namespace:
            // optional. easier to read to move around namespaces
            .Replace(
                $"PipelineNamespacePlaceholder.", 
                $"{PipelineEmitter.NamespacePrefix}.");

        return normalized;
    }
}
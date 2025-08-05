namespace Skybeam.Tests.Models;

public class FileDescription
{
    public FileDescription(string relativePath, string generatedFileName)
    {
        RelativePath = relativePath;
        GeneratedFileName = generatedFileName;
    }

    public string RelativePath { get; init; }
    public string GeneratedFileName { get; init; }
}
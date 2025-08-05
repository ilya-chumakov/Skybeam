using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Skybeam.Tests.Helpers;

public class CompilationHelper
{
    public static void AssertCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>();

        var compilation = CSharpCompilation.Create(
            "InMemoryAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (result.Success)
        {
            Assert.True(true, "Compilation succeeded.");
        }
        else
        {
            var sb = new StringBuilder();

            sb.AppendLine("Compilation failed. Errors:");
            foreach (var diagnostic in result.Diagnostics
                         .Where(d => d.Severity == DiagnosticSeverity.Error))
            {
                sb.AppendLine(diagnostic.ToString());
            }
            string message = sb.ToString();

            Assert.Fail(message);
        }
    }
}
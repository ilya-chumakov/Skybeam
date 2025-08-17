using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Skybeam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public static class CompilationHelperV2
{
    public static Assembly RunGenerator(IIncrementalGenerator generator, IEnumerable<string> sourceCodes)
    {
        var syntaxTrees = sourceCodes.Select(code => CSharpSyntaxTree.ParseText(code));

        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        assemblies.Add(typeof(AddSkybeamExtension).Assembly);

        var references = assemblies
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>();

        var compilation = CSharpCompilation.Create(
            "GeneratorTestAssembly",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver
            .Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);

        if (diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
        {
            throw new InvalidOperationException("Generator produced errors: " +
                string.Join(Environment.NewLine, diagnostics));
        }

        using var ms = new MemoryStream();
        var emitResult = updatedCompilation.Emit(ms);

        if (!emitResult.Success)
        {
            var errors = string.Join(Environment.NewLine,
                emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            throw new InvalidOperationException("Emit failed: " + errors);
        }

        ms.Seek(0, SeekOrigin.Begin);
        return Assembly.Load(ms.ToArray());
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skybeam.Abstractions;
using Skybeam;

namespace Skybeam.Tests.Helpers;

public static class Verifier
{
    public class Test : CSharpSourceGeneratorTest<PipelineGenerator, DefaultVerifier>
    {
        public Test()
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90;

            var refs = new[]
            {
                typeof(IRequestHandler<,>).Assembly,
                typeof(RegistrationExtensions).Assembly,
                typeof(PipelineGenerator).Assembly,
                typeof(ServiceCollectionDescriptorExtensions).Assembly
            };
            foreach (var asm in refs)
            {
                TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(asm.Location));
            }
        }

        public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Default;

        protected override CompilationOptions CreateCompilationOptions()
        {
            var compilationOptions = base.CreateCompilationOptions();
            return compilationOptions.WithSpecificDiagnosticOptions(
                compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));
        }

        protected override ParseOptions CreateParseOptions()
        {
            return ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(LanguageVersion);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Skybeam.Abstractions;
using Skybeam.Tests.Helpers;
using Skybeam.Tests.Models;
using Skybeam.Tests.Snapshots.OneBehavior;

namespace Skybeam.Tests;

/// <summary>
/// End-to-end test, from source generation to running the generated code
/// </summary>
public class PipelineRegistry_Tests
{
    [Fact]
    public async Task Apply_OneBehavior_PipelineAndHandlerAreRegisteredForDI()
    {
        // Arrange
        PipelineGenerator generator = new PipelineGenerator();

        SourceDescription description = new Skybeam.Tests.Snapshots.OneBehavior.SourceDescription();

        List<TestFile> sourceFiles = await SnapshotReader.ReadAsync(description.FolderName, description.SourceFiles);

        string[] sources = sourceFiles.Select(x => x.Content).ToArray();

        Assembly assembly = CompilationHelperV2.RunGenerator(generator, sources);
        
        string sourceNamespace = "Skybeam.Tests.Snapshots.OneBehavior";
        string pipelineNamespace = $"{PipelineTextEmitter.NamespacePrefix}.Skybeam.Tests.Snapshots.OneBehavior";
        
        Type alphaType = assembly.GetType($"{sourceNamespace}.Alpha")!;
        Type omegaType = assembly.GetType($"{sourceNamespace}.Omega")!;
        Type handlerType = assembly.GetType($"{sourceNamespace}.BarHandler")!;
        Type pipelineType = assembly.GetType($"{pipelineNamespace}.BarHandlerPipeline")!;
        Type registryType = assembly.GetType($"{PipelineTextEmitter.NamespacePrefix}.PipelineRegistry")!;
        
        alphaType.Should().NotBeNull();
        omegaType.Should().NotBeNull();
        handlerType.Should().NotBeNull();
        pipelineType.Should().NotBeNull();
        registryType.Should().NotBeNull();
        
        Type closedHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(alphaType, omegaType);
        
        MethodInfo applyMethod = registryType.GetMethod("Apply");
        object registry = Activator.CreateInstance(registryType);
        IServiceCollection services = new ServiceCollection();
        
        // Act
        applyMethod?.Invoke(registry, [services]);
        
        // Assert
        services.Any(d => d.ServiceType == handlerType && d.ImplementationType == handlerType).Should().BeTrue();
        services.Any(d => d.ServiceType == closedHandlerInterfaceType && d.ImplementationType == pipelineType).Should().BeTrue();
    }
}
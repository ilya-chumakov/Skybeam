using System;
using System.Collections.Generic;

namespace Skybeam.Tests.Models;

public abstract class SourceDescriptionBase
{
    public string FolderName => GetFolder(GetType());
    public List<HandlerDescription> Handlers { get; init; } = [];
    public List<BehaviorDescription> Behaviors { get; init; } = [];
    public List<FileDescription> SourceFiles { get; init; } = [];
    public List<FileDescription> ExpectedFiles { get; init; } = [];
    public Dictionary<string, string> ReplaceRules { get; init; } = [];
    
    public static string GetFolder(Type type)
    {
        string segment = type.Namespace?.Split('.')[^1];
        return segment ?? throw new InvalidOperationException($"Cannot determine folder for {type.FullName}.");
    }

    public static FileDescription DefaultSourceFile { get; } = new("Source.cs", null);
    public static FileDescription DefaultExpectedPipeline { get; } = 
        new("ExpectedPipeline.cs", "BarHandler_Pipeline.g.cs");
    public static FileDescription DefaultExpectedRegistry { get; } = 
        new("ExpectedRegistry.cs", "PipelineRegistry.g.cs");

    protected static string GetDisplayFullName<TType>()
    {
        var type = typeof(TType);

        if (type.IsGenericType)
        {
            string name = type.Name;
            int index = name.IndexOf('`');
            string trimmed = index >= 0 ? name.Substring(0, index) : name;

            return "global::" + type.Namespace + "." + trimmed;    
        }

        return "global::" + type.FullName;
    }

    protected void AddDefaultFiles()
    {
        SourceFiles.Add(DefaultSourceFile);
        ExpectedFiles.Add(DefaultExpectedPipeline);
        ExpectedFiles.Add(DefaultExpectedRegistry);
    }
}
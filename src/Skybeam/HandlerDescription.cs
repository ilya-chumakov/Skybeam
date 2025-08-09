namespace Skybeam;

public record HandlerDescription(
    string Name,
    string FullName,
    string ContainingNamespace,
    string InputFullName,
    string OutputFullName,
    string PipelineSuffix = "");

public record BehaviorDescription(string FullNamePrefix);

public record PipelineDescription(
    HandlerDescription HandlerDescription,
    string FullName);
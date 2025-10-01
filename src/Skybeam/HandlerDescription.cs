namespace Skybeam;

internal record HandlerDescription(
    string Name,
    string FullName,
    string ContainingNamespace,
    string InputFullName,
    string OutputFullName,
    string PipelineSuffix = "");

internal record BehaviorDescription(string FullNamePrefix);

internal record PipelineDescription(
    HandlerDescription HandlerDescription,
    string FullName);
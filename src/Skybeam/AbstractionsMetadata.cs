namespace Skybeam;

internal class AbstractionsMetadata
{
    public static AbstractionsMetadata Instance { get; internal set; } = new();

    public const string RequestInterfaceSymbolName = "IRequestHandler";
    public const string BehaviorInterfaceSymbolName = "IPipelineBehavior";
    public const string AssemblySymbolName = "Skybeam.Abstractions";
}
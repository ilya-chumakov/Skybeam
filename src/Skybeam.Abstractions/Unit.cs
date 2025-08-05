namespace Skybeam.Abstractions;

/// <summary>
///     A type that represents a void-like response in a generic context.
///     https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/unit-type
/// </summary>
public readonly struct Unit : IEquatable<Unit>
{
    public static Unit Value => new();
    public static Task<Unit> CompletedTask => Task.FromResult(Value);
    public override bool Equals(object obj) => obj is Unit;
    public bool Equals(Unit other) => true;
    public override int GetHashCode() => 0;
    public override string ToString() => "()";
}
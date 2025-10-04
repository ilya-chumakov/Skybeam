using Skybeam.Abstractions;

namespace WebApiRoot.Domain;

public record ControllerInput(int Id);
public record ControllerOutput(int Id, string Name);
public class ControllerHandler : IRequestHandler<ControllerInput, ControllerOutput>
{
    public Task<ControllerOutput> HandleAsync(ControllerInput input, CancellationToken ct = default)
    {
        return Task.FromResult(new ControllerOutput(42, nameof(ControllerHandler)));
    }
}
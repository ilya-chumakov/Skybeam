namespace Skybeam.Playground;

public interface IRequestHandler<in TInput, TOutput>
{
    Task<TOutput> HandleAsync(TInput input, CancellationToken ct = default);
}
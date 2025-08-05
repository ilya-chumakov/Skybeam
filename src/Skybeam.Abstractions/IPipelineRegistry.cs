using Microsoft.Extensions.DependencyInjection;

namespace Skybeam.Abstractions;

public interface IPipelineRegistry
{
    public void Apply(IServiceCollection services);
}
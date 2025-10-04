using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skybeam.Abstractions;

namespace Skybeam;

// shallow checks, room for improvement
internal class RegistrationVerifier(DelayedLog log)
{
    private static readonly Type RequestHandlerType = typeof(IRequestHandler<,>);

    public void VerifyRegistryCount(IReadOnlyCollection<Type> registryTypes)
    {
        if (registryTypes.Count == 1)
        {
            return;
        }
        else if (registryTypes.Count == 0)
        {
            log.Add(logger =>
                logger.LogWarning("No registry was found. Check if the source generator has run."));
        }
        else
        {
            string types = string.Join("," + Environment.NewLine, registryTypes.Select(x => x.ToString()));

            string message =
                "More than one registry was found. Check for multiple registry implementations. Found the types:"
                + Environment.NewLine
                + types;

            log.Add(logger => logger.LogWarning(message));
        }
    }

    public void VerifyServiceRegistrations(IServiceCollection services)
    {
        bool anyPipelineIsFound = services.Any(sd =>
            sd.ServiceType.IsGenericType &&
            sd.ServiceType.GetGenericTypeDefinition() == RequestHandlerType);

        if (!anyPipelineIsFound)
        {
            log.Add(logger =>
                logger.LogWarning("No pipelines registered. Check if a request handler is declared. " +
                                  "Check the generated registry's code to ensure the handler is found."));
        }

        Type[] duplicateInputTypes = services
            .Where(sd => sd.ServiceType.IsGenericType &&
                         sd.ServiceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
            .Select(sd => sd.ServiceType.GetGenericArguments()[0])
            .GroupBy(type => type)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        if (duplicateInputTypes.Any())
        {
            string types = string.Join("," + Environment.NewLine, duplicateInputTypes.Select(x => x.ToString()));

            string message =
                "More than one handler with the same input type was found. " +
                "While it would work, using an input only once is strongly recommended for clarity and readability. " +
                "Found the types:"
                + Environment.NewLine
                + types;

            log.Add(logger => logger.LogWarning(message));
        }
    }

    public void VerifyRegistry(IPipelineRegistry registry)
    {
        //nothing to verify now
    }
}
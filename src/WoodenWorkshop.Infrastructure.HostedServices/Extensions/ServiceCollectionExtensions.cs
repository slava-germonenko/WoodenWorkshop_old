using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using WoodenWorkshop.Infrastructure.HostedServices.Runners;

namespace WoodenWorkshop.Infrastructure.HostedServices.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddScopedTimedHostedService<TService>(
        this IServiceCollection services, 
        TimeSpan sleepTime
    ) where TService : class, IScopedHostedService
    {
        services.TryAddScoped<ITimedServiceRunnerSettings<TService>>(
            _ => new TimedServiceRunnerSettings<TService>(sleepTime)
        );

        services.TryAddScoped<TService>();
        services.AddHostedService<TimedScopedHostedServiceRunner<TService>>();
    }
}
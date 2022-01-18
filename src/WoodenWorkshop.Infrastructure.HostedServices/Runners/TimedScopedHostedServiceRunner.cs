using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WoodenWorkshop.Infrastructure.HostedServices.Runners;

public class TimedScopedHostedServiceRunner<TService> : BackgroundService where TService : IScopedHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;


    public TimedScopedHostedServiceRunner(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunScopedService(stoppingToken);
            await Task.Delay(GetRunnerSleepTime(), stoppingToken);
        }
    }

    private TimeSpan GetRunnerSleepTime()
    {
        using var scope = _scopeFactory.CreateScope();
        return scope.ServiceProvider.GetService<ITimedServiceRunnerSettings<TService>>()?.SleepTime
               ?? throw new Exception($"Missing settings for {typeof(TService).Name} service runner.");
    }

    private async Task RunScopedService(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var serviceToRun = scope.ServiceProvider.GetService<TService>() 
                           ?? throw new Exception($"Unable to resolve service of type {typeof(TService).Name}.");
        try
        {
            await serviceToRun.ExecuteAsync(stoppingToken);
        }
        catch (Exception)
        {
            // Add logging here
        }
    }
}
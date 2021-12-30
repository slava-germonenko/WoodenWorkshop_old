using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using WoodenWorkshop.Auth.Jobs.Settings;
using WoodenWorkshop.Auth.Services.Abstractions;

namespace WoodenWorkshop.Auth.Jobs;

public class ExpireRefreshTokenBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;


    public ExpireRefreshTokenBackgroundService(
        IServiceScopeFactory serviceScopeFactory
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = GetServiceSettings();
        while (!stoppingToken.IsCancellationRequested)
        {
            await ExpireTokens();
            await Task.Delay(settings.SleepTime, stoppingToken);
        }
    }

    private async Task ExpireTokens()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var sessionsService = scope.ServiceProvider.GetService<ISessionsService>();
        var settings = scope.ServiceProvider.GetService<IExpireRefreshTokensSettings>();
        if (sessionsService is null || settings is null)
        {
            throw new Exception("ExpireRefreshTokenBackgroundService: Session service or service settings are not injected.");
        }
        
        var sessionsToExpire = await sessionsService.GetSessionsToExpire(
            settings.ProcessTokensLimit
        );
        await sessionsService.ExpireSessions(sessionsToExpire);
    }

    private IExpireRefreshTokensSettings GetServiceSettings()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var settings = scope.ServiceProvider.GetService<IExpireRefreshTokensSettings>();
        if (settings is null)
        {
            throw new Exception("ExpireRefreshTokenBackgroundService: service settings are not injected.");
        }

        return settings;
    }
}
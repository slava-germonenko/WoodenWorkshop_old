using Microsoft.Extensions.Options;
using WoodenWorkshop.Auth.Jobs.Settings;
using WoodenWorkshop.Crm.Api.Options;

namespace WoodenWorkshop.Crm.Api.Settings;

public class ExpireRefreshTokensSettings : IExpireRefreshTokensSettings
{
    public TimeSpan SleepTime => TimeSpan.FromMinutes(
        _infrastructureSettings.Value.ExpireRefreshTokenIntervalMinutes
    );

    public int ProcessTokensLimit => _infrastructureSettings.Value.ExpireRefreshTokenProcessLimit;

    private readonly IOptionsSnapshot<Infrastructure> _infrastructureSettings;

    public ExpireRefreshTokensSettings(IOptionsSnapshot<Infrastructure> infrastructureOptions)
    {
        _infrastructureSettings = infrastructureOptions;
    }
}
using Microsoft.Extensions.Options;

using WoodenWorkshop.Auth.Jobs.Settings;

namespace WoodenWorkshop.Crm.Api.Settings;

public class ExpireRefreshTokensSettings : IExpireRefreshTokensSettings
{
    public TimeSpan SleepTime => TimeSpan.FromMinutes(
        _infrastructureSettings.Value.ExpireRefreshTokenIntervalMinutes
    );

    public int ProcessTokensLimit => _infrastructureSettings.Value.ExpireRefreshTokenProcessLimit;

    private readonly IOptionsSnapshot<Options.Infrastructure> _infrastructureSettings;

    public ExpireRefreshTokensSettings(IOptionsSnapshot<Options.Infrastructure> infrastructureOptions)
    {
        _infrastructureSettings = infrastructureOptions;
    }
}
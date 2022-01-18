using Microsoft.Extensions.Options;

using WoodenWorkshop.Auth.HostedServices.Settings;

namespace WoodenWorkshop.Crm.Api.Settings;

public class ExpireTokenServiceSettings : IExpireTokenServiceSettings
{
    private readonly IOptionsSnapshot<Options.Infrastructure> _infrastructureOptions;

    public int ExpireRefreshTokenProcessLimit => _infrastructureOptions.Value.ExpireRefreshTokenProcessLimit;

    public ExpireTokenServiceSettings(IOptionsSnapshot<Options.Infrastructure> infrastructureOptions)
    {
        _infrastructureOptions = infrastructureOptions;
    }
}
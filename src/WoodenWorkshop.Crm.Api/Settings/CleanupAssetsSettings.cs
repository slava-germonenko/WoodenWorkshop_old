using Microsoft.Extensions.Options;
using WoodenWorkshop.Core.Assets.HostedServices.Settings;

namespace WoodenWorkshop.Crm.Api.Settings;

public class CleanupAssetsSettings : IAssetsAndFoldersCleanupSettings
{
    private readonly IOptionsSnapshot<Options.Infrastructure> _infrastructureOptions;

    public int ProcessLimit => _infrastructureOptions.Value.RemoveAssetsAndFoldersProcessLimit;

    public CleanupAssetsSettings(IOptionsSnapshot<Options.Infrastructure> infrastructureOptions)
    {
        _infrastructureOptions = infrastructureOptions;
    }
}
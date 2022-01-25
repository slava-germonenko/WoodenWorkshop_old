using WoodenWorkshop.Core.Assets.Services.Abstractions;

namespace WoodenWorkshop.Core.Assets.Services;

public class AssetsCleanupService : IAssetsCleanupService
{
    private readonly IAssetsService _assetsService;


    public AssetsCleanupService(IAssetsService assetsService)
    {
        _assetsService = assetsService;
    }

    public async Task CleanUpAssetsAsync(IEnumerable<Guid> assetIds)
    {
        foreach (var assetId in assetIds)
        {
            await _assetsService.RemoveAssetAsync(assetId);
        }
    }
}
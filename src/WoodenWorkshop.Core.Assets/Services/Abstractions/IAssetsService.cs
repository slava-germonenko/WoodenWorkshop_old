using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services.Abstractions;

public interface IAssetsService
{
    public Task<Asset> GetAssetAsync(Guid assetId);

    public Task<Asset> UpdateAssetAsync(Asset assetToUpdate, bool renameDuplicate);
}
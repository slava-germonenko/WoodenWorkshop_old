using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Assets.Dtos;
using WoodenWorkshop.Core.Assets.Models;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services.Abstractions;

public interface IAssetsListService
{
    Task<PagedCollection<Asset>> GetAssetsListAsync(Page page, AssetsFilter assetsFilter);

    Task<Asset> AddAssetAsync(AddAssetDto assetDto, Stream assetFileStream, bool renameDuplicate);

    Task RemoveAssetAsync(Guid assetId);
}
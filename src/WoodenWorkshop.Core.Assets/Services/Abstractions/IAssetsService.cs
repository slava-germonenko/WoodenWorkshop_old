using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Assets.Dtos;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services.Abstractions;

public interface IAssetsService
{
    Task<Asset> CreateAssetAsync(CreateAssetDto assetDto);

    Task<Asset> GetAssetAsync(Guid id);
    
    Task<PagedCollection<Asset>> GetAssetsAsync(Page page, Guid? folderId);

    Task MarkAsQueuedForRemovalAsync(Guid assetId);

    Task RemoveAssetAsync(Guid assetId);

    Task<Asset> UpdateAssetDetailsAsync(Asset assetToUpdate);

    Task<Asset> UpdateAssetBlobAsync(Guid assetId, Stream file);
}
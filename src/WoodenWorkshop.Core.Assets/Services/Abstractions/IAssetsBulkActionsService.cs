using WoodenWorkshop.Core.Assets.Dtos;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services.Abstractions;

public interface IAssetsBulkActionsService
{
    Task<ICollection<Asset>> UploadAsync(ICollection<CreateAssetDto> assetDtos);
    
    Task QueueForRemovalAsync(IEnumerable<Guid> assetIds);
}
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services.Abstractions;

public interface IAssetsCleanupService
{
    Task<PagedCollection<Asset>> GetAssetsQueuedForRemovalAsync(Page page, Guid? folderId);
}
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services.Abstractions;

public interface IFoldersCleanupService
{
    Task<PagedCollection<Folder>> GetFoldersQueuedForRemovalAsync(Page page, Guid? parentFolderId);
}
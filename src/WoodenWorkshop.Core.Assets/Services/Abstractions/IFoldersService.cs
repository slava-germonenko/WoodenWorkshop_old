using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services.Abstractions;

public interface IFoldersService
{
    Task<PagedCollection<Folder>> GetFoldersAsync(Page page, Guid? parentFolderId = null);

    Task<Folder> CreateAsync(Folder folder);

    Task<Folder> UpdateDetailsAsync(Folder folder);

    Task RemoveFolder(Guid folderId, bool forceRemoveQueuedForRemoval = false);

    Task QueueForRemovalAsync(Guid folderId);
}
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services.Abstractions;

public interface IFoldersService
{
    Task<PagedCollection<Folder>> GetFoldersAsync(
        Page page,
        Guid? parentFolderId = null,
        bool includeQueuedForRemoval = false
    );

    Task<Folder> CreateAsync(Folder folder);

    Task<Folder> UpdateDetailsAsync(Folder folder);

    Task RemoveFolder(Guid folderId);

    Task QueueForRemovalAsync(Guid folderId);
}
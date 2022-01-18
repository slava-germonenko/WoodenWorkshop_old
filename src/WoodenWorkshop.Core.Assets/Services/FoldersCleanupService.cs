using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services;

public class FoldersCleanupService : IFoldersCleanupService
{
    private readonly CoreContext _context;


    public FoldersCleanupService(CoreContext context)
    {
        _context = context;
    }


    public async Task<PagedCollection<Folder>> GetFoldersQueuedForRemovalAsync(Page page, Guid? parentFolderId)
    {
        return await _context.Folders.AsTracking()
            .Where(folder => folder.ParentFolderId == parentFolderId && folder.QueuedForRemoval)
            .OrderBy(folder => folder.Updated)
            .ToPagedCollectionAsync(page);
    }
}
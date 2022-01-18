using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services;

public class AssetsCleanupService : IAssetsCleanupService
{
    private readonly CoreContext _context;


    public AssetsCleanupService(CoreContext context)
    {
        _context = context;
    }


    public async Task<PagedCollection<Asset>> GetAssetsQueuedForRemovalAsync(Page page, Guid? folderId)
    {
        return await _context.Assets.AsNoTracking()
            .Where(asset => asset.FolderId == folderId && asset.QueuedForRemoval)
            .OrderBy(asset => asset.Updated)
            .ToPagedCollectionAsync(page);
    }
}
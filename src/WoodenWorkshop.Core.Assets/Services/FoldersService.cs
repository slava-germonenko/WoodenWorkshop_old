using Azure.Messaging.ServiceBus;

using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Assets.Enums;
using WoodenWorkshop.Core.Assets.Extensions;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services;

public class FoldersService : IFoldersService
{
    private readonly CoreContext _context;

    private readonly ServiceBusClient _serviceBusClient;


    public FoldersService(CoreContext context, ServiceBusClient serviceBusClient)
    {
        _context = context;
        _serviceBusClient = serviceBusClient;
    }


    public async Task<PagedCollection<Folder>> GetFoldersAsync(Page page, Guid? parentFolderId = null)
    {
        return await _context.Folders.AsNoTracking()
            .Where(folder => folder.ParentFolderId == parentFolderId && !folder.QueuedForRemoval)
            .OrderBy(folder => folder.Name)
            .ToPagedCollectionAsync(page);
    }

    public async Task<Folder> CreateAsync(Folder folder)
    {
        await EnsureFolderExists(folder.ParentFolderId);
        await EnsureFolderNameIsNotInUse(folder.Name, folder.ParentFolderId);
        _context.Folders.Add(folder);
        await _context.SaveChangesAsync();
        return folder;
    }

    public async Task<Folder> UpdateDetailsAsync(Folder folder)
    {
        await EnsureFolderExists(folder.ParentFolderId);
        await EnsureFolderNameIsNotInUse(folder.Name, folder.ParentFolderId);
        _context.Folders.Update(folder);
        await _context.SaveChangesAsync();
        return folder;
    }

    public async Task RemoveFolder(Guid folderId, bool forceRemoveQueuedForRemoval = false)
    {
        var folder = await _context.Folders.FirstOrDefaultAsync(
            folder => folder.Id == folderId
        );

        if (folder is null || folder.QueuedForRemoval && !forceRemoveQueuedForRemoval)
        {
            return;
        }

        _context.Folders.Remove(folder);
        await _context.SaveChangesAsync();
    }

    public async Task QueueForRemovalAsync(Guid folderId)
    {
        await MarkFolderAssetsAsQueuedForRemoval(folderId);
        await MarFolderChildFoldersAsQueuedForRemoval(folderId);
        await _serviceBusClient.CreateSender(QueueNames.CleanupFolders)
            .SendFoldersCleanupMessage(folderId);
    }

    private async Task EnsureFolderNameIsNotInUse(string name, Guid? parentFolderId)
    {
        var folderNameInUse = await _context.Folders.AnyAsync(
            folder => folder.ParentFolderId == parentFolderId && folder.Name == name && !folder.QueuedForRemoval
        );
        if (folderNameInUse)
        {
            throw new DuplicateException(Errors.FolderNameInUse(name));
        }
    }
    
    private async Task EnsureFolderExists(Guid? folderId)
    {
        if (folderId is null)
        {
            return;
        }

        var folderExists = await _context.Folders.AnyAsync(
            folder => folder.Id == folderId && !folder.QueuedForRemoval
        );

        if (!folderExists)
        {
            throw new NotFoundException(Errors.FolderNotFound(folderId.Value));
        }
    }

    private async Task MarFolderChildFoldersAsQueuedForRemoval(Guid folderId)
    {
        await _context.Database.ExecuteSqlRawAsync(@"
;WITH FoldersBase AS (
    SELECT Id, ParentFolderId FROM dbo.Folders
    WHERE Id = @FolderId
    UNION ALL
    SELECT F.Id, F.ParentFolderId FROM dbo.Folders F
    INNER JOIN FoldersBase ON FoldersBase.Id = F.ParentFolderId
)
UPDATE dbo.Folders SET QueuedForRemoval = 1 WHERE Id IN (SELECT Id FROM FoldersBase)
", folderId);
    }

    private async Task MarkFolderAssetsAsQueuedForRemoval(Guid folderId)
    {
        await _context.Database.ExecuteSqlRawAsync(@"
;WITH FoldersBase AS (
    SELECT Id, ParentFolderId FROM dbo.Folders
    WHERE Id = @FolderId
    UNION ALL
    SELECT F.Id, F.ParentFolderId FROM dbo.Folders F
    INNER JOIN FoldersBase ON FoldersBase.Id = F.ParentFolderId
)
UPDATE dbo.Assets SET QueuedForRemoval = 1 WHERE FolderId IN (SELECT Id FROM FoldersBase)
", folderId);
    }
}
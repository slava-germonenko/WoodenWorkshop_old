using Azure.Messaging.ServiceBus;

using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Core.Assets.Enums;
using WoodenWorkshop.Core.Assets.Extensions;
using WoodenWorkshop.Core.Assets.Services.Abstractions;

namespace WoodenWorkshop.Core.Assets.Services;

public class FoldersBulkActionsService : IFoldersBulkActionsService
{
    private readonly CoreContext _context;

    private readonly ServiceBusClient _serviceBusClient;


    public FoldersBulkActionsService(CoreContext context, ServiceBusClient serviceBusClient)
    {
        _context = context;
        _serviceBusClient = serviceBusClient;
    }


    public async Task QueuedForRemovalAsync(List<Guid> folderIds)
    {
        await MarkFolderAssetsAsQueuedForRemoval(folderIds);
        await MarFolderChildFoldersAsQueuedForRemoval(folderIds);
        await _serviceBusClient.CreateSender(QueueNames.CleanupFolders)
            .SendFoldersCleanupMessage(folderIds);
    }
    
    private async Task MarFolderChildFoldersAsQueuedForRemoval(IEnumerable<Guid> folderIds)
    {
        var folderIdsString = string.Join(",", folderIds.Select(f => $"'{f.ToString()}'")); 
        await _context.Database.ExecuteSqlRawAsync($@"
;WITH FoldersBase AS (
    SELECT Id, ParentFolderId FROM dbo.Folders
    WHERE Id IN ({folderIdsString})
    UNION ALL
    SELECT F.Id, F.ParentFolderId FROM dbo.Folders F
    INNER JOIN FoldersBase ON FoldersBase.Id = F.ParentFolderId
)
UPDATE dbo.Folders SET QueuedForRemoval = 1 WHERE Id IN (SELECT Id FROM FoldersBase)
");
    }

    private async Task MarkFolderAssetsAsQueuedForRemoval(IEnumerable<Guid> folderIds)
    {
        var folderIdsString = string.Join(",", folderIds.Select(f => $"'{f.ToString()}'"));
        await _context.Database.ExecuteSqlRawAsync($@"
;WITH FoldersBase AS (
    SELECT Id, ParentFolderId FROM dbo.Folders
    WHERE Id IN ({folderIdsString})
    UNION ALL
    SELECT F.Id, F.ParentFolderId FROM dbo.Folders F
    INNER JOIN FoldersBase ON FoldersBase.Id = F.ParentFolderId
)
UPDATE dbo.Assets SET QueuedForRemoval = 1 WHERE FolderId IN (SELECT Id FROM FoldersBase)
");
    }
}
namespace WoodenWorkshop.Core.Assets.Services.Abstractions;

public interface IFoldersBulkActionsService
{
    Task QueuedForRemovalAsync(IEnumerable<Guid> folderIds);
}
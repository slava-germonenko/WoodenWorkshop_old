namespace WoodenWorkshop.Core.Assets.Services.Abstractions;

public interface IFoldersCleanupService
{
    Task CleanUpFoldersAsync(IEnumerable<Guid> folderIds);
}
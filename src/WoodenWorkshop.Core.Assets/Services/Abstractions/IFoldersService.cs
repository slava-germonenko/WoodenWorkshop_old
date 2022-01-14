using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services.Abstractions;

public interface IFoldersService
{
    Task<AssetFolder> GetFolderAsync(Guid folderId);

    Task<AssetFolder> UpdateFolderAsync(AssetFolder folderToUpdate);

    Task RemoveFolderAsync(Guid folderId);
}
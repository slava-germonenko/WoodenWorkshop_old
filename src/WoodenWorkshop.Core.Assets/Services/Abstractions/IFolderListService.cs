using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Assets.Dtos;
using WoodenWorkshop.Core.Assets.Models;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services.Abstractions;

public interface IFolderListService
{
    Task<PagedCollection<AssetFolder>> GetFoldersListAsync(Page page, FolderFilter? folderFilter = null);

    Task<AssetFolder> AddFolderAsync(AddFolderDto folderDto);

    Task RemoveFolderAsync(Guid folderId);
}
using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Assets.Dtos;
using WoodenWorkshop.Core.Assets.Enums;
using WoodenWorkshop.Core.Assets.Models;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services;

public class FolderListService : IFolderListService
{
    private readonly CoreContext _coreContext;


    public FolderListService(CoreContext coreContext)
    {
        _coreContext = coreContext;
    }


    public async Task<PagedCollection<AssetFolder>> GetFoldersListAsync(Page page, FolderFilter? folderFilter = null)
    {
        var foldersQuery = _coreContext.AssetFolders.AsNoTracking();
        
        if (folderFilter?.ParentId is not null)
        {
            await EnsureFolderExists(folderFilter.ParentId.Value);
            foldersQuery = foldersQuery.Where(folder => folder.ParentFolderId == folderFilter.ParentId);
        }

        if (folderFilter?.FileName is not null)
        {
            foldersQuery = foldersQuery.Where(folder => folder.Name.Contains(folderFilter.FileName));
        }
        foldersQuery = foldersQuery.OrderBy(folder => folder.Name);

        return await foldersQuery.ToPagedCollectionAsync(page);
    }

    public async Task<AssetFolder> AddFolderAsync(AddFolderDto folderDto)
    {
        if (folderDto.ParentFolderId is not null)
        {
            await EnsureFolderExists(folderDto.ParentFolderId.Value);
        }

        var folderNameIsInUse = await _coreContext.AssetFolders.AnyAsync(
            folder => folder.Name == folderDto.Name && folder.ParentFolderId == folderDto.ParentFolderId
        );
        if (folderNameIsInUse)
        {
            throw new DuplicateException($"Директория с именем {folderDto.Name} уже существует.");
        }

        var asset = new AssetFolder
        {
            Name = folderDto.Name,
            ParentFolderId = folderDto.ParentFolderId,
        };
        await _coreContext.AssetFolders.AddAsync(asset);
        await _coreContext.SaveChangesAsync();

        return await _coreContext.AssetFolders.FindOrNotFoundExceptionAsync(asset.Id);
    }

    public async Task RemoveFolderAsync(Guid folderId)
    {
        var folder = await _coreContext.AssetFolders.FindAsync(folderId);
        if (folder is not null)
        {
            _coreContext.AssetFolders.Remove(folder);
            await _coreContext.SaveChangesAsync();
        }
    }

    private async Task EnsureFolderExists(Guid folderId)
    {
        var folderExists = await _coreContext.AssetFolders.AnyAsync(folder => folder.Id == folderId);
        if (!folderExists)
        {
            throw new NotFoundException(Errors.FolderNotFound(folderId));
        }
    }
}
using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Core.Assets.Enums;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services;

public class FoldersService : IFoldersService
{
    private readonly CoreContext _context;


    public FoldersService(CoreContext context)
    {
        _context = context;
    }


    public async Task<AssetFolder> GetFolderAsync(Guid folderId)
    {
        return await _context.AssetFolders.FindOrNotFoundExceptionAsync(folderId);
    }

    public async Task<AssetFolder> UpdateFolderAsync(AssetFolder folderToUpdate)
    {
        var folderNameIsInUse = await _context.AssetFolders.AnyAsync(
            folder => folder.Name == folderToUpdate.Name 
                      && folder.ParentFolderId == folderToUpdate.ParentFolderId
                      && folder.Id != folderToUpdate.Id
        );
        if (folderNameIsInUse)
        {
            throw new DuplicateException(Errors.FolderAlreadyExists(folderToUpdate.Name));
        }

        _context.AssetFolders.Update(folderToUpdate);
        await _context.SaveChangesAsync();
        return await GetFolderAsync(folderToUpdate.Id);
    }

    public async Task RemoveFolderAsync(Guid folderId)
    {
        var folder = await _context.AssetFolders.FindAsync(folderId);
        if (folder is not null)
        {
            _context.AssetFolders.Remove(folder);
            await _context.SaveChangesAsync();
        }
    }
}
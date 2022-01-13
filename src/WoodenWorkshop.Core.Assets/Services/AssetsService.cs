using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Core.Assets.Enums;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services;

public class AssetsService : IAssetsService
{
    private readonly CoreContext _context;


    public AssetsService(CoreContext context)
    {
        _context = context;
    }


    public async Task<Asset> GetAssetAsync(Guid assetId)
    {
        return await _context.Assets.FindOrNotFoundExceptionAsync(assetId, Errors.AssetNotFound(assetId));
    }

    public async Task<Asset> UpdateAssetAsync(Asset assetToUpdate, bool renameDuplicate)
    {
        if (assetToUpdate.FolderId is not null)
        {
            await EnsureFolderExistsAsync(assetToUpdate.FolderId.Value);
        }
        
        var assetNameIsInUse = await _context.Assets.AnyAsync(
            a => a.AssetName == assetToUpdate.AssetName && assetToUpdate.FolderId == a.FolderId && a.Id != assetToUpdate.Id
        );

        if (assetNameIsInUse && !renameDuplicate)
        {
            throw new DuplicateException(Errors.AssetAlreadyExists(assetToUpdate.AssetName));
        }

        if (assetNameIsInUse)
        {
            assetToUpdate.AssetName = $"(Копия) {assetToUpdate.AssetName}";
        }
        _context.Assets.Update(assetToUpdate);
        await _context.SaveChangesAsync();
        return await GetAssetAsync(assetToUpdate.Id);
    }

    private async Task EnsureFolderExistsAsync(Guid folderId)
    {
        var folderExists = await _context.AssetFolders.AnyAsync(folder => folder.Id == folderId);
        if (!folderExists)
        {
            throw new NotFoundException(Errors.FolderNotFound(folderId));
        }
    }
}
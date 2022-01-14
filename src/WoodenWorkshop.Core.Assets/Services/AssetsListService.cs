using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Assets.Dtos;
using WoodenWorkshop.Core.Assets.Enums;
using WoodenWorkshop.Core.Assets.Models;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Models.Enums;

namespace WoodenWorkshop.Core.Assets.Services;

public class AssetsListService : IAssetsListService
{
    private readonly IAssetsBlobClientFactory _blobClientFactory;

    private readonly CoreContext _context;


    public AssetsListService(IAssetsBlobClientFactory blobClientFactory, CoreContext context)
    {
        _blobClientFactory = blobClientFactory;
        _context = context;
    }


    public async Task<PagedCollection<Asset>> GetAssetsListAsync(Page page, AssetsFilter? assetsFilter = null)
    {
        var assetsQuery = _context.Assets.AsTracking();
        
        if (assetsFilter?.FolderId is not null)
        {
            assetsQuery = assetsQuery.Where(asset => asset.FolderId == assetsFilter.FolderId);
        }
        if (assetsFilter?.FileName is not null)
        {
            assetsQuery = assetsQuery.Where(asset => asset.AssetName.Contains(assetsFilter.FileName));
        }
        assetsQuery = assetsQuery.OrderBy(asset => asset.AssetName);

        return await assetsQuery.ToPagedCollectionAsync(page);
    }

    public async Task<Asset> AddAssetAsync(AddAssetDto assetDto, Stream fileStream, bool renameDuplicate)
    {
        if (assetDto.FolderId is not null)
        {
            await EnsureFolderExistsAsync(assetDto.FolderId.Value);
        }
        
        var assetNameIsInUse = await _context.Assets.AnyAsync(
            a => a.AssetName == assetDto.OriginalFileName && a.FolderId == assetDto.FolderId
        );
        if (assetNameIsInUse && !renameDuplicate)
        {
            throw new DuplicateException(Errors.AssetAlreadyExists(assetDto.OriginalFileName));
        }

        var assetFileName = assetNameIsInUse ? $"(Копия) {assetDto.OriginalFileName}" : assetDto.OriginalFileName;
        var assetFileExt = Path.GetExtension(assetFileName).Trim('.');
        var asset = new Asset
        {
            AssetType = DetermineAssetType(assetFileExt),
            AssetName = assetFileName,
            FolderId = assetDto.FolderId,
        };
        await _context.Assets.AddAsync(asset);
        await _context.SaveChangesAsync();

        var blobClient = _blobClientFactory.CreateAssetBlobClient($"{asset.Id.ToString()}.{assetFileExt}");
        await blobClient.UploadAsync(fileStream);

        asset.BlobName = blobClient.Name;
        asset.Url = blobClient.Uri;
        _context.Assets.Update(asset);
        await _context.SaveChangesAsync();

        return await _context.Assets.FindOrNotFoundExceptionAsync(asset.Id);
    }

    public async Task RemoveAssetAsync(Guid assetId)
    {
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset?.BlobName is null)
        {
            return;
        }

        var blobClient = _blobClientFactory.CreateAssetBlobClient(asset.BlobName);
        await blobClient.DeleteIfExistsAsync();
        
        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();
    }

    private AssetType DetermineAssetType(string fileExt)
    {
        if (!Asset.SupportedFileExtensions.Contains(fileExt, StringComparer.InvariantCultureIgnoreCase))
        {
            throw new ValidationException(Errors.AssetFormatNotSupported(fileExt));
        }

        return Asset.FileExtensionToAssetTypeMap[fileExt];
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
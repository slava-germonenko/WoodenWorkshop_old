using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;

using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Assets.Dtos;
using WoodenWorkshop.Core.Assets.Enums;
using WoodenWorkshop.Core.Assets.Extensions;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services;

public class AssetsService : IAssetsService
{
    private const string AssetsContainerName = "assets";

    private readonly BlobServiceClient _blobServiceClient;

    private readonly CoreContext _context;
    
    private readonly ServiceBusClient _serviceBusClient;


    public AssetsService(
        BlobServiceClient blobServiceClient,
        CoreContext context,
        ServiceBusClient serviceBusClient
   )
    {
        _blobServiceClient = blobServiceClient;
        _context = context;
        _serviceBusClient = serviceBusClient;
    }

    public async Task<Asset> GetAssetAsync(Guid id)
    {
        return await _context.Assets.FindOrNotFoundExceptionAsync(id, Errors.AssetNotFound(id));
    }

    public async Task<PagedCollection<Asset>> GetAssetsAsync(
        Page page,
        Guid? folderId = null,
        bool includeQueuedForRemoval = false
    )
    {
        var baseQuery = _context.Assets.AsNoTracking()
            .Where(asset => asset.FolderId == folderId);

        if (!includeQueuedForRemoval)
        {
            baseQuery = baseQuery.Where(asset => !asset.QueuedForRemoval);
        }

        return await baseQuery.OrderBy(asset => asset.AssetName).ToPagedCollectionAsync(page);
    }

    public async Task<Asset> CreateAssetAsync(CreateAssetDto assetDto)
    {
        await EnsureFolderExists(assetDto.FolderId);

        var fileExtension = Path.GetExtension(assetDto.AssetName).Trim('.').ToLower();
        if (!Asset.SupportedFileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            throw new ValidationException(Errors.AssetTypeUnsupported(fileExtension));
        }

        var assetId = Guid.NewGuid();
        var blobName = $"{assetId.ToString()}.{fileExtension}";
        var assetsContainerName = _blobServiceClient.GetBlobContainerClient(AssetsContainerName);

        await assetsContainerName.UploadBlobAsync(blobName, assetDto.FileStream);
        var asset = new Asset
        {
            Id = assetId,
            AssetName = assetDto.AssetName,
            BlobName = blobName,
            Url = assetsContainerName.GetBlobClient(blobName).Uri,
            Size = assetDto.FileStream.Length,
            FolderId = assetDto.FolderId,
        };
        await _context.Assets.AddAsync(asset);
        await _context.SaveChangesAsync();

        return await GetAssetAsync(assetId);
    }

    public async Task RemoveAssetAsync(Guid assetId)
    {
        var asset = await _context.Assets.FindAsync(assetId);

        if (asset is null)
        {
            return;
        }

        var blobClient = _blobServiceClient.GetBlobContainerClient(AssetsContainerName)
            .GetBlobClient(asset.BlobName);

        await blobClient.DeleteIfExistsAsync();
        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();
    }
    
    public async Task<Asset> UpdateAssetDetailsAsync(Asset assetToUpdate)
    {
        var asset = await _context.Assets.FirstOrNotFoundExceptionAsync(
            asset => asset.Id == assetToUpdate.Id,
            Errors.AssetNotFound(assetToUpdate.Id)
        );

        await EnsureFolderExists(assetToUpdate.FolderId);
        
        asset.FolderId = assetToUpdate.FolderId;
        asset.AssetName = assetToUpdate.AssetName;

        _context.Assets.Update(asset);
        await _context.SaveChangesAsync();
        return asset;
    }

    public async Task MarkAsQueuedForRemovalAsync(Guid assetId)
    {
        var asset = await _context.Assets.FirstOrDefaultAsync(asset => asset.Id == assetId && !asset.QueuedForRemoval);
        if (asset is null)
        {
            return;
        }
        
        asset.QueuedForRemoval = true;
        asset.FolderId = null;
        _context.Assets.Update(asset);
        await _context.SaveChangesAsync();

        await _serviceBusClient.CreateSender(QueueNames.CleanupAssets)
            .SendAssetsCleanupMessage(assetId);
    }
    
    public async Task<Asset> UpdateAssetBlobAsync(Guid assetId, Stream file)
    {
        var asset = _context.Assets.FirstOrNotFoundException(
            a => a.Id == assetId && !a.QueuedForRemoval,
            Errors.AssetNotFound(assetId)
        );

        var blobClient = _blobServiceClient.GetBlobContainerClient(AssetsContainerName)
            .GetBlobClient(asset.BlobName);
        await blobClient.UploadAsync(file);

        asset.Size = file.Length;
        await _context.SaveChangesAsync();
        return asset;
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
}
using Azure.Messaging.ServiceBus;

using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Assets.Enums;
using WoodenWorkshop.Core.Assets.Extensions;
using WoodenWorkshop.Core.Assets.Services.Abstractions;

namespace WoodenWorkshop.Core.Assets.Services;

public class FoldersCleanupService : IFoldersCleanupService
{
    private readonly IAssetsService _assetsService;

    private readonly IFoldersService _foldersService;

    private readonly ServiceBusClient _serviceBusClient;

    private readonly Page _allItemsPage = new(){Size = int.MaxValue, Index = 0};

    private const int CleanupAssetsMessageLen = 100;


    public FoldersCleanupService(
        IAssetsService assetsService,
        IFoldersService foldersService,
        ServiceBusClient serviceBusClient
    )
    {
        _assetsService = assetsService;
        _foldersService = foldersService;
        _serviceBusClient = serviceBusClient;
    }


    public async Task CleanUpFoldersAsync(IEnumerable<Guid> folderIds)
    {
        foreach (var folderId in folderIds)
        {
            await RemoveFolderContentAsync(folderId);
        }
    }

    private async Task RemoveFolderContentAsync(Guid folderId)
    {
        var childFolders = await _foldersService.GetFoldersAsync(_allItemsPage, folderId, true);
        foreach (var folder in childFolders.Items)
        {
            await RemoveFolderContentAsync(folder.Id);
        }

        var folderAssets = await _assetsService.GetAssetsAsync(_allItemsPage, folderId, true);
        foreach (var asset in folderAssets.Items)
        {
            asset.FolderId = null;
            asset.QueuedForRemoval = true;
            await _assetsService.UpdateAssetDetailsAsync(asset);
        }

        await _foldersService.RemoveFolder(folderId);
        
        await using var sender = _serviceBusClient.CreateSender(QueueNames.CleanupAssets);
        for (var offset = 0; offset < folderAssets.Items.Count; offset += CleanupAssetsMessageLen)
        {
            var assetsBatch = folderAssets.Items
                .Skip(offset)
                .Take(CleanupAssetsMessageLen)
                .Select(asset => asset.Id)
                .ToList();

            await sender.SendAssetsCleanupMessage(assetsBatch);
        }
    }
}
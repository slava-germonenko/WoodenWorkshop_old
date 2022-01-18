using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Assets.HostedServices.Settings;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Infrastructure.HostedServices;

namespace WoodenWorkshop.Core.Assets.HostedServices;

public class AssetsAndFoldersCleanupService : IScopedHostedService
{
    private readonly IAssetsAndFoldersCleanupSettings _settings;

    private readonly IAssetsCleanupService _assetsCleanupService;
    
    private readonly IAssetsService _assetsService;

    private readonly IFoldersCleanupService _foldersCleanupService;

    private readonly IFoldersService _foldersService;

    private Page DefaultPage => new() {Size = _settings.ProcessLimit, Index = 0};


    public AssetsAndFoldersCleanupService(
        IAssetsAndFoldersCleanupSettings settings,
        IAssetsCleanupService assetsCleanupService,
        IAssetsService assetsService,
        IFoldersCleanupService foldersCleanupService,
        IFoldersService foldersService
    )
    {
        _settings = settings;
        _assetsCleanupService = assetsCleanupService;
        _assetsService = assetsService;
        _foldersCleanupService = foldersCleanupService;
        _foldersService = foldersService;
    }


    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await RemoveFolderContentQueuedForRemoval(null);
    }

    public async Task RemoveFolderContentQueuedForRemoval(Guid? folderId)
    {
        var childFolders = await _foldersCleanupService.GetFoldersQueuedForRemovalAsync(DefaultPage, folderId);
        while (childFolders.Total > 0)
        {
            foreach (var folder in childFolders.Items)
            {
                await RemoveFolderContentQueuedForRemoval(folder.Id);
            }

            childFolders = await _foldersService.GetFoldersAsync(DefaultPage, folderId);
        }

        await RemoveAssets(folderId);
        if (folderId.HasValue)
        {
            await _foldersService.RemoveFolder(folderId.Value, true);
        }
    }

    private async Task RemoveAssets(Guid? folderId)
    {
        var assets = await _assetsCleanupService.GetAssetsQueuedForRemovalAsync(DefaultPage, folderId);
        while (assets.Total > 0)
        {
            foreach (var assetsItem in assets.Items)
            {
                await _assetsService.RemoveAssetAsync(assetsItem.Id);
            }

            assets = await _assetsCleanupService.GetAssetsQueuedForRemovalAsync(DefaultPage, folderId);
        }
    }
}
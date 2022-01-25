using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using WoodenWorkshop.Core.Assets.Dtos;
using WoodenWorkshop.Core.Assets.Enums;
using WoodenWorkshop.Core.Assets.Extensions;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services;

public class AssetsBulkActionsService : IAssetsBulkActionsService
{
    private readonly CoreContext _context;

    private readonly ServiceBusClient _serviceBusClient;
    
    private readonly IServiceScopeFactory _serviceScopeFactory;


    public AssetsBulkActionsService(
        CoreContext context,
        ServiceBusClient serviceBusClient,
        IServiceScopeFactory scopeFactory
    )
    {
        _context = context;
        _serviceBusClient = serviceBusClient;
        _serviceScopeFactory = scopeFactory;
    }


    public async Task<ICollection<Asset>> UploadAsync(ICollection<CreateAssetDto> assetDtos)
    {
        var uploadAssetsTasks = new List<Task<Asset>>(assetDtos.Count);
        foreach (var assetDto in assetDtos)
        {
            uploadAssetsTasks.Add(UploadAssetAsync(assetDto));
        }

        return await Task.WhenAll(uploadAssetsTasks);
    }

    public async Task QueueForRemovalAsync(IEnumerable<Guid> assetIds)
    {
        var assetsQuery = await _context.Assets
            .Where(asset => assetIds.Contains(asset.Id))
            .ToListAsync();
        assetsQuery.ForEach(asset =>
        {
            asset.FolderId = null;
            asset.QueuedForRemoval = true;
        });
        await _context.SaveChangesAsync();
        await _serviceBusClient.CreateSender(QueueNames.CleanupAssets)
            .SendAssetsCleanupMessage(
                assetIds.ToList()
            );
    }

    private async Task<Asset> UploadAssetAsync(CreateAssetDto assetDto)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var assetsService = scope.ServiceProvider.GetRequiredService<IAssetsService>();
        return await assetsService.CreateAssetAsync(assetDto);
    }
}
using Microsoft.Extensions.DependencyInjection;

using WoodenWorkshop.Core.Assets.Dtos;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Assets.Services;

public class AssetsBulkActionsService : IAssetsBulkActionsService
{
    private readonly CoreContext _context;

    private readonly IServiceScopeFactory _serviceScopeFactory;


    public AssetsBulkActionsService(CoreContext context, IServiceScopeFactory scopeFactory)
    {
        _context = context;
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
        var assetsQuery = _context.Assets.Where(asset => assetIds.Contains(asset.Id));
        _context.Assets.RemoveRange(assetsQuery);
        await _context.SaveChangesAsync();
    }

    private async Task<Asset> UploadAssetAsync(CreateAssetDto assetDto)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var assetsService = scope.ServiceProvider.GetRequiredService<IAssetsService>();
        return await assetsService.CreateAssetAsync(assetDto);
    }
}
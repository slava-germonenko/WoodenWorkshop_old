using System.Text.Json;

using Microsoft.Azure.Functions.Worker;

using WoodenWorkshop.Core.Assets.Dtos;
using WoodenWorkshop.Core.Assets.Services.Abstractions;

namespace WoodenWorkshop.Crm.Functions.Triggers;

public class CleanupAssets
{
    private readonly IAssetsCleanupService _assetsCleanupService;
        

    public CleanupAssets(IAssetsCleanupService assetsCleanupService)
    {
        _assetsCleanupService = assetsCleanupService;
    }

    [Function("CleanupAssets")]
    public async Task Run([ServiceBusTrigger("assets-cleanup")] string messageBody)
    {
        var cleanupAssetsMessage = JsonSerializer.Deserialize<CleanupAssetsMessage>(messageBody);
        if (cleanupAssetsMessage is not null)
        {
            Console.WriteLine(cleanupAssetsMessage.AssetIds.Count);
            await _assetsCleanupService.CleanUpAssetsAsync(cleanupAssetsMessage.AssetIds);
        }
    }
}
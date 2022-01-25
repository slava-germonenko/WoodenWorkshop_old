using System.Text.Json;

using Azure.Messaging.ServiceBus;

using WoodenWorkshop.Core.Assets.Dtos;

namespace WoodenWorkshop.Core.Assets.Extensions;

public static class ServiceBusSenderExtensions
{
    public static async Task SendAssetsCleanupMessage(this ServiceBusSender sender, Guid assetId)
    {
        await sender.SendAssetsCleanupMessage(new List<Guid> {assetId});
    }

    public static async Task SendAssetsCleanupMessage(this ServiceBusSender sender, List<Guid> assetIds)
    {
        var cleanupAssetsMessage = new CleanupAssetsMessage(assetIds);
        var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(cleanupAssetsMessage));
        await sender.SendMessageAsync(serviceBusMessage);
    }

    public static async Task SendFoldersCleanupMessage(this ServiceBusSender sender, Guid folderId)
    {
        await sender.SendFoldersCleanupMessage(new List<Guid> {folderId});
    }
    
    public static async Task SendFoldersCleanupMessage(this ServiceBusSender sender, List<Guid> folderIds)
    {
        var cleanupAssetsMessage = new CleanupFoldersMessage(folderIds);
        var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(cleanupAssetsMessage));
        await sender.SendMessageAsync(serviceBusMessage);
    }
}
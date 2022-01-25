namespace WoodenWorkshop.Core.Assets.Services.Abstractions;

public interface IAssetsCleanupService
{
    Task CleanUpAssetsAsync(IEnumerable<Guid> assetIds);
}
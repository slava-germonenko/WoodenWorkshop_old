using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Products.Services.Abstractions;

public interface IProductAssetsService
{
    Task<ProductAsset> SaveProductAssetAsync(ProductAsset productAsset);

    Task SetProductAssetsOrderAsync(Guid productId, IList<Guid> orderedProductAssetIds);
    
    Task RemoveProductAssetAsync(Guid productAssetId);
}
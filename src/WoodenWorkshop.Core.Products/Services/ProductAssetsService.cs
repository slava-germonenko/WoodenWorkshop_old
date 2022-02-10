using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Products.Services.Abstractions;

namespace WoodenWorkshop.Core.Products.Services;

public class ProductAssetsService : IProductAssetsService
{
    private readonly CoreContext _context;


    public ProductAssetsService(CoreContext context)
    {
        _context = context;
    }


    public async Task<ProductAsset> SaveProductAssetAsync(ProductAsset sourceProductAsset)
    {
        var productAssetToSave = await _context.ProductAssets.FindAsync(sourceProductAsset.Id)
                                 ?? new ProductAsset{ProductId = sourceProductAsset.ProductId};
        productAssetToSave.CopyDetails(sourceProductAsset);

        _context.Update(productAssetToSave);
        await _context.SaveChangesAsync();
        return productAssetToSave;
    }

    public async Task SetProductAssetsOrderAsync(Guid productId, IList<Guid> orderedProductAssetIds)
    {
        await _context.Products.AnyOrNotFoundExceptionAsync(
            product => product.Id == productId && !product.IsDeleted,
            Errors.ProductNotFound(productId)
        );

        var productAssets = await _context.ProductAssets.Where(asset => asset.ProductId == productId).ToListAsync();
        var productAssetIds = productAssets.Select(asset => asset.Id).ToList();
        
        var missingProductAssetIds = productAssetIds.Except(orderedProductAssetIds).ToList();
        if (missingProductAssetIds.Any())
        {
            throw new ValidationException(Errors.MissingProductAssets(productId, missingProductAssetIds));
        }

        var redundantProductAssetIds = orderedProductAssetIds.Except(productAssetIds).ToList();
        if (redundantProductAssetIds.Any())
        {
            throw new ValidationException(Errors.RedundantProductAssets(productId, redundantProductAssetIds));
        }
        
        productAssets.ForEach(asset =>
        {
            var newAssetIndex = orderedProductAssetIds.IndexOf(asset.Id);
            asset.AssetOrder = newAssetIndex + 1;
        });

        await _context.SaveChangesAsync();
    }

    public async Task RemoveProductAssetAsync(Guid productAssetId)
    {
        var productAssetToRemove = await _context.ProductAssets.FindAsync(productAssetId);
        if (productAssetToRemove is null)
        {
            return;
        }

        var productAssetsToMove = await _context.ProductAssets
            .Where(a => a.ProductId == productAssetToRemove.Id && a.AssetOrder > productAssetToRemove.AssetOrder)
            .ToListAsync();
        
        productAssetsToMove.ForEach(asset => asset.AssetOrder--);

        _context.Remove(productAssetToRemove);
        await _context.SaveChangesAsync();
    }
}
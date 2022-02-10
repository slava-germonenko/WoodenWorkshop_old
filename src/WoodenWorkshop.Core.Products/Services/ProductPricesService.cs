using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Products.Services.Abstractions;

namespace WoodenWorkshop.Core.Products.Services;

public class ProductPricesService : IProductPricesService
{
    private readonly CoreContext _context;


    public ProductPricesService(CoreContext context)
    {
        _context = context;
    }


    public async Task<ProductPrice> SaveProductPriceAsync(ProductPrice sourceProductPrice)
    {
        var productId = sourceProductPrice.ProductId;
        var priceTypeId = sourceProductPrice.PriceTypeId;
        var productPriceToSave = await _context.ProductPrices.FirstOrDefaultAsync(
            price => price.ProductId == productId && price.PriceTypeId == priceTypeId 
        ) ?? new ProductPrice{PriceTypeId = priceTypeId, ProductId = productId};

        productPriceToSave.Value = sourceProductPrice.Value;

        _context.Update(productPriceToSave);
        await _context.SaveChangesAsync();
        return productPriceToSave;
    }

    public async Task RemoveProductPriceAsync(Guid productId, Guid priceTypeId)
    {
        var productPriceToRemove = await _context.ProductPrices.FirstOrDefaultAsync(
            price => price.ProductId == productId && price.PriceTypeId == productId
        );

        if (productPriceToRemove is not null)
        {
            _context.ProductPrices.Remove(productPriceToRemove);
            await _context.SaveChangesAsync();
        }
    }
}
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Products.Services.Abstractions;

public interface IProductPricesService
{
    Task<ProductPrice> SaveProductPriceAsync(ProductPrice sourceProductPrice);

    Task RemoveProductPriceAsync(Guid productId, Guid priceTypeId);
}
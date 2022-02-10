using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Products.Models;

namespace WoodenWorkshop.Core.Products.Services.Abstractions;

public interface IProductsService
{
    Task<PagedCollection<ProductThumbnail>> GetProductsPageAsync(
        Page page,
        ProductsFilter? productsFilter = null,
        OrderByQuery? orderByQuery = null
    );

    Task<Product> GetProductAsync(Guid productId);

    Task<Product> SaveProductDetailsAsync(Product product);

    Task SoftDeleteProductAsync(Guid productId);
}
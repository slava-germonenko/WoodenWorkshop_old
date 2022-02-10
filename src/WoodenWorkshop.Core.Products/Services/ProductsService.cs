using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Products.Extensions;
using WoodenWorkshop.Core.Products.Models;
using WoodenWorkshop.Core.Products.Services.Abstractions;

namespace WoodenWorkshop.Core.Products.Services;

public class ProductsService : IProductsService
{
    private readonly CoreContext _context;


    public ProductsService(CoreContext context)
    {
        _context = context;
    }


    public async Task<PagedCollection<ProductThumbnail>> GetProductsPageAsync(
        Page page,
        ProductsFilter? productsFilter = null,
        OrderByQuery? orderByQuery = null
    )
    {
        var productsQuery = _context.Products.Where(product => !product.IsDeleted).AsNoTracking();
        if (productsFilter is not null)
        {
            productsQuery = productsQuery.ApplyProductsFilter(productsFilter);
        }

        productsQuery = orderByQuery is null
            ? productsQuery.OrderByDescending(product => product.Created)
            : productsQuery.ApplyOrderClause(new ProductsOrderByClause(orderByQuery));

        return await productsQuery.ProjectToProductThumbnail().ToPagedCollectionAsync(page);
    }

    public async Task<Product> GetProductAsync(Guid productId)
    {
        return await _context.Products.AsNoTracking()
            .Include(product => product.Assets)
            .Include(product => product.Category)
            .Include(product => product.Material)
            .Include(product => product.ProductPrices)
            .ThenInclude(price => price.PriceType)
            .Include(product => product.ProductSocialLinks)
            .FirstOrNotFoundExceptionAsync(
                product => product.Id == productId && !product.IsDeleted,
                Errors.ProductNotFound(productId)
            );
    }

    public async Task<Product> SaveProductDetailsAsync(Product sourceProduct)
    {
        await _context.Products.NoneOrDuplicateExceptionAsync(
            product => product.VendorCode == sourceProduct.VendorCode && product.Id != sourceProduct.Id,
            Errors.ProductVendorCodeUnavailable(sourceProduct.VendorCode)
        );

        await _context.Products.NoneOrDuplicateExceptionAsync(
            product => product.Id == sourceProduct.Id && product.IsDeleted
        );

        var productToSave = await _context.Products.FirstOrDefaultAsync(product => product.Id == sourceProduct.Id)
                            ?? new Product{IsDeleted = false};
        productToSave.CopyDetails(sourceProduct);

        _context.Update(productToSave);
        await _context.SaveChangesAsync();

        return productToSave;
    }

    public async Task SoftDeleteProductAsync(Guid productId)
    {
        var productToRemove = await _context.Products.FindOrNotFoundExceptionAsync(productId);
        productToRemove.MarkAsDeleted();

        _context.Update(productToRemove);
        await _context.SaveChangesAsync();
    }
}
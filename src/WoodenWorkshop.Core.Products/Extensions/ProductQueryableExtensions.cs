using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Products.Models;

namespace WoodenWorkshop.Core.Products.Extensions;

public static class ProductQueryableExtensions
{
    public static IQueryable<Product> ApplyProductsFilter(
        this IQueryable<Product> productsQuery,
        ProductsFilter productsFilter
    )
    {
        var resultQuery = productsQuery;
        if (!string.IsNullOrEmpty(productsFilter.Search))
        {
            resultQuery = resultQuery.Where(product => 
                product.RussianName.Contains(productsFilter.Search)
                || product.EnglishName.Contains(productsFilter.Search)
                || product.VendorCode.Contains(productsFilter.Search)
            );
        }

        if (productsFilter.Status is not null)
        {
            resultQuery = resultQuery.Where(product => product.ItemStatus == productsFilter.Status);
        }

        if (productsFilter.CategoryId is not null)
        {
            resultQuery = resultQuery.Where(product => product.CategoryId == productsFilter.CategoryId);
        }

        if (productsFilter.MaterialId is not null)
        {
            resultQuery = resultQuery.Where(product => product.MaterialId == productsFilter.MaterialId);
        }

        if (productsFilter.IsActive is not null)
        {
            resultQuery = resultQuery.Where(product => product.IsActive == productsFilter.IsActive);
        }

        return resultQuery;
    }

    public static IQueryable<ProductThumbnail> ProjectToProductThumbnail(this IQueryable<Product> productsQuery)
    {
        return productsQuery.Select(product => new ProductThumbnail
        {
            RussianName = product.RussianName,
            EnglishName = product.EnglishName,
            VendorCode = product.VendorCode,
            IsActive = product.IsActive,
            Category = product.Category,
            Material = product.Material,
            Size = new ProductSize
            {
                Height = product.Height,
                Width = product.Width,
                Depth = product.Depth,
            },
            Status = product.ItemStatus,
        });
    }
}
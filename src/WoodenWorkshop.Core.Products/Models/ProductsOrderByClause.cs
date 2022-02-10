using System.Linq.Expressions;

using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Products.Models;

public record ProductsOrderByClause(OrderByQuery OrderByQuery) : OrderByClause<Product>(OrderByQuery)
{
    private readonly Expression<Func<Product, object>> _defaultKeyPicker = product => product.Created;
    
    private readonly Dictionary<string, Expression<Func<Product, object>>> _orderParametersMap =
        new(StringComparer.InvariantCultureIgnoreCase)
        {
            {"RussianName", product => product.RussianName},
            {"EnglishName", product => product.EnglishName},
            {"VendorCode", product => product.VendorCode},
            {"Category", product => product.Category.Name},
            {"Material", product => product.Material.Name},
            {"Created", product => product.Created},
        };
    
    private Expression<Func<Product, object>>? _keyPicker;

    public override Expression<Func<Product, object>> KeyPicker
    {
        get
        {
            if (string.IsNullOrEmpty(OrderByQuery.OrderBy))
            {
                return _defaultKeyPicker;
            }
            _orderParametersMap.TryGetValue(OrderByQuery.OrderBy, out _keyPicker);
            return _keyPicker ??= _defaultKeyPicker;
        }
    }
}
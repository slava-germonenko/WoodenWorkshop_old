using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Core.Models.Enums;

namespace WoodenWorkshop.Core.Products.Models;

public record ProductsFilter(
    string? Search,
    Guid? MaterialId,
    Guid? CategoryId,
    bool? IsActive,
    ProductItemStatus? Status
) : BaseFilter(Search);
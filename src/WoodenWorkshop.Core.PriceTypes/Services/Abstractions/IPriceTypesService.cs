using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.PriceTypes.Services.Abstractions;

public interface IPriceTypesService
{
    Task<PagedCollection<PriceType>> GetPriceTypesAsync(Page page, BaseFilter? filter = null);

    Task<PriceType> SavePriceTypeAsync(PriceType sourcePriceType);

    Task RemovePriceTypeAsync(Guid priceTypeId);
}
using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.PriceTypes.Services.Abstractions;

namespace WoodenWorkshop.Core.PriceTypes.Services;

public class PriceTypesService : IPriceTypesService
{
    private readonly CoreContext _context;


    public PriceTypesService(CoreContext context)
    {
        _context = context;
    }


    public async Task<PagedCollection<PriceType>> GetPriceTypesAsync(Page page, BaseFilter? filter = null)
    {
        var priceTypesQuery = _context.PriceTypes.AsNoTracking();
        priceTypesQuery = string.IsNullOrEmpty(filter?.Search)
            ? priceTypesQuery.OrderByDescending(p => p.Created)
            : priceTypesQuery.Where(p => p.Name.Contains(filter.Search));

        return await priceTypesQuery.ToPagedCollectionAsync(page);
    }

    public async Task<PriceType> SavePriceTypeAsync(PriceType sourcePriceType)
    {
        var priceTypeNameIsInUse = await _context.PriceTypes.AnyAsync(
            p => p.Name == sourcePriceType.Name && p.Id != sourcePriceType.Id
        );
        if (priceTypeNameIsInUse)
        {
            throw new DuplicateException(Errors.PriceTypeNameIsInUse(sourcePriceType.Name));
        }

        var priceType = await _context.PriceTypes.FindAsync(sourcePriceType.Id) ?? new PriceType();
        priceType.CopyDetails(sourcePriceType);

        _context.PriceTypes.Update(priceType);
        await _context.SaveChangesAsync();

        return priceType;
    }

    public async Task RemovePriceTypeAsync(Guid priceTypeId)
    {
        await _context.Database.ExecuteSqlRawAsync(@"
            BEGIN TRANSACTION [T1]
            BEGIN TRY
                DELETE FROM [dbo].[ProductPrices] WHERE PriceTypeId = @PriceTypeId
                DELETE FROM [dbo].[PriceTypes] WHERE Id = @PriceTypeId
                COMMIT TRANSACTION [T1]
            END TRY
            BEGIN CATCH
                ROLLBACK TRANSACTION [T1]
            END CATCH
        ", priceTypeId);
    }
}
using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Materials.Services.Abstractions;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Materials.Services;

public class MaterialsService : IMaterialsService
{
    private readonly CoreContext _context;


    public MaterialsService(CoreContext context)
    {
        _context = context;
    }


    public async Task<PagedCollection<Material>> GetMaterialsAsync(Page page, BaseFilter? filter)
    {
        var materialsQuery = _context.Materials.AsNoTracking();
        materialsQuery = string.IsNullOrEmpty(filter?.Search)
            ? materialsQuery.OrderByDescending(m => m.Created)
            : materialsQuery.Where(m => m.Name.Contains(filter.Search)).OrderBy(m => m.Name);

        return await materialsQuery.ToPagedCollectionAsync(page);
    }

    public async Task<Material> SaveMaterialAsync(Material sourceMaterial)
    {
        var materialNameIsUnUse = await _context.Materials.AnyAsync(
            m => m.Name == sourceMaterial.Name && m.Id != sourceMaterial.Id
        );

        if (materialNameIsUnUse)
        {
            throw new DuplicateException(Errors.MaterialNameIsInUse(sourceMaterial.Name));
        }

        var materialToSave = await _context.Materials.FindAsync(sourceMaterial.Id) ?? new Material();
        materialToSave.CopyDetails(sourceMaterial);

        _context.Materials.Update(materialToSave);
        await _context.SaveChangesAsync();

        return materialToSave;
    }

    public async Task RemoveMaterialAsync(Guid materialToRemoveId)
    {
        await _context.Database.ExecuteSqlRawAsync(@$"
            BEGIN TRANSACTION
            BEGIN TRY
                DECLARE @MaterialToRemoveId UNIQUEIDENTIFIER = '{materialToRemoveId}'
                UPDATE [dbo].[Products] SET MaterialId = NULL WHERE MaterialId = @MaterialToRemoveId
                DELETE FROM [dbo].[Materials] WHERE Id = @MaterialToRemoveId
                COMMIT TRANSACTION
            END TRY
            BEGIN CATCH
                ROLLBACK TRANSACTION
                THROW
            END CATCH
        ");
    }
}
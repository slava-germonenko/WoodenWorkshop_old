using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Materials.Services.Abstractions;

public interface IMaterialsService
{
    Task<PagedCollection<Material>> GetMaterialsAsync(Page page, BaseFilter? filter);

    Task<Material> SaveMaterialAsync(Material sourceMaterial);

    Task RemoveMaterialAsync(Guid materialToRemoveId);
}
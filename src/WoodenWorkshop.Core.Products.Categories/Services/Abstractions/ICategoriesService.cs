using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Products.Categories.Services.Abstractions;

public interface ICategoriesService
{
    Task<PagedCollection<Category>> GetCategoriesAsync(Page page, BaseFilter? filter = null);

    Task<Category> SaveCategoryAsync(Category sourceCategory);

    Task RemoveCategoryAsync(Guid categoryToRemoveId, Guid? categoryToReassignItemsId);
}
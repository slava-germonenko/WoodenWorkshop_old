using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Products.Categories.Services.Abstractions;

namespace WoodenWorkshop.Core.Products.Categories.Services;

public class CategoriesService : ICategoriesService
{
    private readonly CoreContext _context;


    public CategoriesService(CoreContext context)
    {
        _context = context;
    }


    public async Task<PagedCollection<Category>> GetCategoriesAsync(Page page, BaseFilter? filter = null)
    {
        var categoriesQuery = _context.Categories.AsNoTracking();
        
        categoriesQuery = string.IsNullOrEmpty(filter?.Search)
            ? categoriesQuery.OrderByDescending(c => c.Created)
            : categoriesQuery.Where(c => c.Name.Contains(filter.Search)).OrderBy(c => c.Name);

        return await categoriesQuery.ToPagedCollectionAsync(page);
    }

    public async Task<Category> SaveCategoryAsync(Category sourceCategory)
    {
        var categoryNameInUse = await _context.Categories.AnyAsync(
            c => c.Id != sourceCategory.Id && c.Name == sourceCategory.Name
        );
        if (categoryNameInUse)
        {
            throw new DuplicateException(Errors.CategoryNameIsInUse(sourceCategory.Name));
        }
        
        var categoryToSave = await _context.Categories.FindAsync(sourceCategory.Id) ?? new Category();
        categoryToSave.CopyDetails(sourceCategory);
        _context.Categories.Update(categoryToSave);
        await _context.SaveChangesAsync();
        return categoryToSave;
    }

    public async Task RemoveCategoryAsync(Guid categoryToRemoveId, Guid? categoryToReassignItemsId)
    {
        await _context.Categories.EnsureExistsAsync(
            c => c.Id == categoryToRemoveId,
            Errors.CategoryNotFound(categoryToRemoveId)
        );

        if (categoryToReassignItemsId is not null)
        {
            await _context.Categories.EnsureExistsAsync(
                c => c.Id == categoryToReassignItemsId, 
                Errors.CategoryNotFound(categoryToReassignItemsId.Value)
            );
        }

        var reassignProductsSql = categoryToReassignItemsId is null
            ? $"UPDATE [dbo].[Products] SET CategoryId = NULL WHERE CategoryId = {categoryToRemoveId}"
            : $"UPDATE [dbo].[Products] SET CategoryId = {categoryToReassignItemsId.Value} WHERE CategoryId = {categoryToRemoveId}";

        await _context.Database.ExecuteSqlRawAsync(@$"
            BEGIN TRANSACTION [T1]
                BEGIN TRY
                    {reassignProductsSql}
                    DELETE FROM [dbo].[Categories] WHERE Id = {categoryToRemoveId}
                    COMMIT TRANSACTION [T1]
                END TRY
            BEGIN CATCH
                ROLLBACK TRANSACTION [T1]
        ");
    }
}
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;

namespace WoodenWorkshop.Common.Extensions;

public static class QueryableExtensions
{
    public static async Task AnyOrNotFoundExceptionAsync<TSource>(
        this IQueryable<TSource> query, 
        Expression<Func<TSource, bool>> predicate,
        string message = "Ничего не найдено по данному запросу."
    )
    {
        var exists = await query.AnyAsync(predicate);
        if (!exists)
        {
            throw new NotFoundException(message);
        }
    }
    
    public static async Task NoneOrDuplicateExceptionAsync<TSource>(
        this IQueryable<TSource> query, 
        Expression<Func<TSource, bool>> predicate,
        string message = "Запись уже существует."
    )
    {
        var exists = await query.AnyAsync(predicate);
        if (exists)
        {
            throw new DuplicateException(message);
        }
    }
    
    public static TSource FirstOrNotFoundException<TSource>(
        this IQueryable<TSource> query, 
        Expression<Func<TSource, bool>> predicate,
        string message = "Ничего не найдено по данному запросу."
    )
    {
        var result = query.FirstOrDefault(predicate);
        if (result is null)
        {
            throw new NotFoundException(message);
        }

        return result;
    }
    
    public static async Task<TSource> FirstOrNotFoundExceptionAsync<TSource>(
        this IQueryable<TSource> query, 
        Expression<Func<TSource, bool>> predicate,
        string message = "Ничего не найдено по данному запросу."
    )
    {
        var result = await query.FirstOrDefaultAsync(predicate);
        if (result is null)
        {
            throw new NotFoundException(message);
        }

        return result;
    }

    public static IOrderedQueryable<TSource> ApplyOrderClause<TSource>(
        this IQueryable<TSource> query,
        OrderByClause<TSource> orderByClause
    )
    {
        return orderByClause.IsDesc
            ? query.OrderByDescending(orderByClause.KeyPicker)
            : query.OrderBy(orderByClause.KeyPicker);
    }

    public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> query, Page page)
    {
        return query.Skip(page.Size * page.Index).Take(page.Size);
    }

    public static async Task<PagedCollection<TSource>> ToPagedCollectionAsync<TSource>(
        this IQueryable<TSource> query,
        Page page
    )
    {
        var items = await query.Page(page).ToListAsync();
        var itemsCount = await query.CountAsync();
        return new PagedCollection<TSource>(page, items, itemsCount);
    }
    
    public static IQueryable<TSource> WhereNotNull<TSource, TValue>(
        this IQueryable<TSource> queryable,
        Expression<Func<TSource, bool>> expression,
        TValue? value
    )
    {
        return value is null
            ? queryable
            : queryable.Where(expression);
    }
}
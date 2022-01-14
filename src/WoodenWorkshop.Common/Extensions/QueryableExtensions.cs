using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Models.Paging;

namespace WoodenWorkshop.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Page<T>(this IQueryable<T> query, Page page)
    {
        return query.Skip(page.Size * page.Index).Take(page.Size);
    }

    public static PagedCollection<TSource> ToPagedCollection<TSource>(
        this IQueryable<TSource> query,
        Page page
    )
    {
        var items = query.Page(page).ToList();
        var itemsCount = query.Count();
        return new PagedCollection<TSource>(page, items, itemsCount);
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
}
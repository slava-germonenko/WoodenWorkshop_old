using System.Linq.Expressions;
using WoodenWorkshop.Common.Models.Paging;

namespace WoodenWorkshop.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Page<T>(this IQueryable<T> query, Page page)
    {
        return query.Skip(page.Size * page.Index).Take(page.Size);
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
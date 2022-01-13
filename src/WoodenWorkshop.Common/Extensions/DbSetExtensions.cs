using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;

namespace WoodenWorkshop.Common.Extensions;

public static class DbSetExtensions
{
    public static TSource FindOrNotFoundException<TSource>(
        this DbSet<TSource> dbSet,
        object pk,
        string message = "Ничего не найдено по данному запросу."
    ) where TSource : class
    {
        var entity = dbSet.Find(pk);
        if (entity is null)
        {
            throw new NotFoundException(message);
        }

        return entity;
    }
    
    public static async Task<TSource> FindOrNotFoundExceptionAsync<TSource>(
        this DbSet<TSource> dbSet,
        object pk,
        string message = "Ничего не найдено по данному запросу."
    ) where TSource : class
    {
        var entity = await dbSet.FindAsync(pk);
        if (entity is null)
        {
            throw new NotFoundException(message);
        }

        return entity;
    }
}
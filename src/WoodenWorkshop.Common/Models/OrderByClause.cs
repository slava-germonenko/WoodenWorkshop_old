using System.Linq.Expressions;

namespace WoodenWorkshop.Common.Models;

public abstract record OrderByClause<TModel>(OrderByQuery OrderByQuery)
{
    public bool IsDesc => OrderByQuery.IsDesc;

    public abstract Expression<Func<TModel, object>> KeyPicker { get; }
}
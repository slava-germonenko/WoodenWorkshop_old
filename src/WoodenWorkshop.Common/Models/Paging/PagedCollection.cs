namespace WoodenWorkshop.Common.Models.Paging;

public class PagedCollection<TItem>
{
    public int Total { get; }
    
    public Page Page { get; }

    public IReadOnlyCollection<TItem> Items { get; }

    public PagedCollection(Page page, IReadOnlyCollection<TItem> items): this(page, items, items.Count) { }

    public PagedCollection(Page page, IReadOnlyCollection<TItem> items, int total)
    {
        Page = page;
        Items = items;
        Total = total;
    }
}
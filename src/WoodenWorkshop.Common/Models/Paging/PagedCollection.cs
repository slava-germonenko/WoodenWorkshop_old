namespace WoodenWorkshop.Common.Models.Paging;

public class PagedCollection<TItem>
{
    public Page Page { get; }

    public IReadOnlyCollection<TItem> Items { get; }

    public PagedCollection(Page page, IReadOnlyCollection<TItem> items)
    {
        Page = page;
        Items = items;
    }
}
namespace WoodenWorkshop.Common.Models;

public record OrderByQuery
{
    public string Direction { get; set; } = "asc";
    
    public string? OrderBy { get; set; }

    public bool IsAsc => string.Equals(Direction, "ASC", StringComparison.OrdinalIgnoreCase);

    public bool IsDesc => string.Equals(Direction, "DESC", StringComparison.OrdinalIgnoreCase);
}
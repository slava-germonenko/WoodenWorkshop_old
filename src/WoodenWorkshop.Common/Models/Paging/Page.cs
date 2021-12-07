using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Common.Models.Paging;

public record Page
{
    public const int DefaultIndex = 0;

    public const int DefaultSize = 50;

    [Range(0, int.MaxValue)]
    public int Index { get; set; } = DefaultIndex;

    [Range(1, int.MaxValue)]
    public int Size { get; set; } = DefaultSize;
}
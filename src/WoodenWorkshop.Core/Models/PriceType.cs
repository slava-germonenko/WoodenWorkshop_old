using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class PriceType : BaseModel
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
}
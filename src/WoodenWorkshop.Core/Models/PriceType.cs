using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class PriceType : BaseModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public void CopyDetails(PriceType source)
    {
        Name = source.Name;
    }
}
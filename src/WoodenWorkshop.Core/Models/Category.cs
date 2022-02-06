using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class Category : BaseModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public void CopyDetails(Category source)
    {
        Name = source.Name;
        IsActive = source.IsActive;
    }
}
namespace WoodenWorkshop.Core.Models;

public class Product : BaseModel
{
    public string RussianName { get; set; }

    public string EnglishName { get; set; }

    public string VendorCode { get; set; }

    public Guid? CategoryId { get; set; }

    public Category? Category { get; set; }

    public ICollection<ProductPrice> Prices { get; set; }

    public ICollection<ProductAsset> Assets { get; set; }
}
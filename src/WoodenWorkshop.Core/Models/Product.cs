namespace WoodenWorkshop.Core.Models;

public class Product : BaseModel
{
    public string Name { get; set; }
    
    public string EnglishName { get; set; }
    
    public string VendorCode { get; set; }
    
    public Guid CreatedById { get; set; }
}
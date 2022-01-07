using WoodenWorkshop.Core.Models.Enums;

namespace WoodenWorkshop.Core.Models;

public class ProductPrice : BaseModel
{
    public PriceTypes PriceType { get; set; }
    
    public decimal Value { get; set; }
}
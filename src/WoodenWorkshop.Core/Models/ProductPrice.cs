namespace WoodenWorkshop.Core.Models;

public class ProductPrice
{
    public Guid ProductId { get; set; }
    
    public Guid PriceTypeId { get; set; }

    public decimal Value { get; set; }

    public PriceType PriceType { get; set; }
}
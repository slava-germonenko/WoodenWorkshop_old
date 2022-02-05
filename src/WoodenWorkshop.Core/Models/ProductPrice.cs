using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class ProductPrice
{
    public Guid PriceTypeId { get; set; }

    public Guid ProductId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Value { get; set; }

    public PriceType PriceType { get; set; }
}
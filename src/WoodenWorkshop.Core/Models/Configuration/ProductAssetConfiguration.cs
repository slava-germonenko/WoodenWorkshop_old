using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WoodenWorkshop.Core.Models.Configuration;

public class ProductAssetConfiguration : IEntityTypeConfiguration<ProductAsset>
{
    public void Configure(EntityTypeBuilder<ProductAsset> builder)
    {
        builder.Property(product => product.Url)
            .HasConversion(
                uri => uri.ToString(), 
                uriString => new Uri(uriString)
            );
    }
}
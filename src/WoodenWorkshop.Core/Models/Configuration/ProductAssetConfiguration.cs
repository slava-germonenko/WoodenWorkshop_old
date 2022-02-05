using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WoodenWorkshop.Core.Models.Configuration;

public class ProductAssetConfiguration : IEntityTypeConfiguration<ProductAssets>
{
    public void Configure(EntityTypeBuilder<ProductAssets> builder)
    {
        builder.Property(pa => pa.Url)
            .HasConversion(uri => uri.ToString(), uriString => new Uri(uriString));
    }
}
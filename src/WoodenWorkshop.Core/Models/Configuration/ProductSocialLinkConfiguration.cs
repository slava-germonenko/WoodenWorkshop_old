using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WoodenWorkshop.Core.Models.Configuration;

public class ProductSocialLinkConfiguration : IEntityTypeConfiguration<ProductSocialLink>
{
    public void Configure(EntityTypeBuilder<ProductSocialLink> builder)
    {
        builder.Property(link => link.Url)
            .HasConversion(uri => uri.ToString(), uriString => new Uri(uriString));
    }
}
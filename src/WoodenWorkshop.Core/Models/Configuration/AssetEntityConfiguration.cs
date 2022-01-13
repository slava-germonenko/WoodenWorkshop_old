using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WoodenWorkshop.Core.Models.Enums;

namespace WoodenWorkshop.Core.Models.Configuration;

public class AssetEntityConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.Property(asset => asset.AssetType)
            .HasConversion<string>(
                assetType => assetType.ToString(),
                assetTypeString => Enum.Parse<AssetType>(assetTypeString)
            );
        
        builder.Property(asset => asset.Url)
            .HasConversion<string?>(
                url => url == null ? null : url.ToString(),
                urlString => urlString == null ? null : new Uri(urlString)
            );
    }
}
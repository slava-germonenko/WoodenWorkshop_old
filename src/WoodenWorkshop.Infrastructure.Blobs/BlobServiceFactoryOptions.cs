using WoodenWorkshop.Infrastructure.Blobs.Abstractions;

namespace WoodenWorkshop.Infrastructure.Blobs;

public record BlobServiceFactoryOptions : IBlobServiceFactoryOptions
{
    public string ConnectionString { get; set; }
}
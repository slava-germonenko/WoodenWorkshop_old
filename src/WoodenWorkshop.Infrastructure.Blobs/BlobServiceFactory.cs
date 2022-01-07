using Azure.Storage.Blobs;

using WoodenWorkshop.Infrastructure.Blobs.Abstractions;

namespace WoodenWorkshop.Infrastructure.Blobs;

public class BlobServiceFactory : IBlobServiceFactory
{
    private readonly IBlobServiceFactoryOptions _options;


    public BlobServiceFactory(IBlobServiceFactoryOptions options)
    {
        _options = options;
    }

    public BlobServiceFactory(BlobServiceFactoryOptions options)
    {
        _options = options;
    }


    public BlobServiceClient CrateBlobServiceClient() => new(_options.ConnectionString);
}
using Azure.Storage.Blobs;

namespace WoodenWorkshop.Infrastructure.Blobs.Abstractions;

public interface IBlobServiceFactory
{
    BlobServiceClient CrateBlobServiceClient();
}
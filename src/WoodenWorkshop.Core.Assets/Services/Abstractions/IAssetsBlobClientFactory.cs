using Azure.Storage.Blobs;

namespace WoodenWorkshop.Core.Assets.Services.Abstractions;

public interface IAssetsBlobClientFactory
{
    BlobClient CreateAssetBlobClient(string blobName);
}
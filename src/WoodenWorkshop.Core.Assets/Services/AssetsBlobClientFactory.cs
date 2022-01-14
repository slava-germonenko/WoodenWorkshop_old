using Azure.Storage.Blobs;

using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Infrastructure.Blobs.Abstractions;

namespace WoodenWorkshop.Core.Assets.Services;

public class AssetsBlobClientFactory : IAssetsBlobClientFactory
{
    private const string AssetsContainerName = "assets";
    
    private readonly IBlobServiceFactory _blobServiceFactory;


    public AssetsBlobClientFactory(IBlobServiceFactory blobServiceFactory)
    {
        _blobServiceFactory = blobServiceFactory;
    }


    public BlobClient CreateAssetBlobClient(string blobName)
    {
        return _blobServiceFactory.CrateBlobServiceClient()
            .GetBlobContainerClient(AssetsContainerName)
            .GetBlobClient(blobName);
    }
}
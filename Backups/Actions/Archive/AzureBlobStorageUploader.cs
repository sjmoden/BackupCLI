using Azure.Storage.Blobs.Models;

namespace Backups.Actions.Archive;

public class AzureBlobStorageUploader : IAzureBlobStorageUploader
{
    private readonly IBlobServiceClientFactory _blobServiceClientFactory;

    public AzureBlobStorageUploader(IBlobServiceClientFactory blobServiceClientFactory) => _blobServiceClientFactory = blobServiceClientFactory;

    public void Upload(UploadParameters uploadParameters)
    {
        var blobServiceClient = _blobServiceClientFactory.Create(uploadParameters.AzureStorageAccount);
        var blobClientContainer = blobServiceClient.GetBlobContainerClient(uploadParameters.Container);
        var blobClient = blobClientContainer.GetBlobClient(uploadParameters.BlobPath);

        blobClient.Upload(uploadParameters.FilePath, new BlobUploadOptions
        {
            AccessTier = AccessTier.Archive
        });
    }
}

public record UploadParameters(string AzureStorageAccount, string Container, string FilePath, string BlobPath);

public interface IAzureBlobStorageUploader
{
    void Upload(UploadParameters uploadParameters);
}
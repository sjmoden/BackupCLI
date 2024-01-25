using Azure.Identity;
using Azure.Storage.Blobs;

namespace Backups.Actions.Archive;

public class BlobServiceClientFactory : IBlobServiceClientFactory
{
    public BlobServiceClient Create(string accountName) => new(new Uri($"https://{accountName}.blob.core.windows.net"), new AzureCliCredential());
}

public interface IBlobServiceClientFactory
{
    BlobServiceClient Create(string accountName);
}
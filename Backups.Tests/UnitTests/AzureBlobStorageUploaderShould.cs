using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Backups.Actions.Archive;
using FakeItEasy;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class AzureBlobStorageUploaderShould
{
    [Test]
    public void Upload_a_file_to_azure()
    {
        var blobServiceClientFactory = A.Fake<IBlobServiceClientFactory>();
        var blobServiceClient = A.Fake<BlobServiceClient>();
        
        const string storageAccountName = "account13";
        A.CallTo(() => blobServiceClientFactory.Create(storageAccountName)).Returns(blobServiceClient);
        
        var blobContainerClient = A.Fake<BlobContainerClient>();
        const string container = "backups";
        A.CallTo(() => blobServiceClient.GetBlobContainerClient(container)).Returns(blobContainerClient);
        
        var blobClient = A.Fake<BlobClient>();
        const string fileName = "file.zip";
        const string filePath = $@"C:\{fileName}";
        const string blobFolder = "file";
        const string blobPath = $@"{blobFolder}\{fileName}";
        A.CallTo(() => blobContainerClient.GetBlobClient(blobPath)).Returns(blobClient);
        
        new AzureBlobStorageUploader(blobServiceClientFactory).Upload(new UploadParameters(storageAccountName, container, filePath, blobPath));

        A.CallTo(() =>
                blobClient.Upload(filePath, A<BlobUploadOptions>.That.Matches(b => b.AccessTier == AccessTier.Archive),
                    A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}
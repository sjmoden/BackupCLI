using System.Reflection;
using Azure.Storage.Blobs;
using Backups.Actions.Archive;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class BlobServiceClientFactoryShould
{
    [Test]
    public void Create_a_blob_service_client()
    {
        const string accountName = "account123";
        var blobServiceClientFactory = new BlobServiceClientFactory();
        var blobServiceClient = blobServiceClientFactory.Create(accountName);
        Assert.That(blobServiceClient.AccountName, Is.EqualTo(accountName));
    }
}
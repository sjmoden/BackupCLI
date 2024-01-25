using Backups.Model;

namespace Backups.Tests.UnitTests.BackupSettingsTests;

[TestFixture]
public class BackupSettingsShould
{
    [TestCaseSource(nameof(_notEqualTestCases))]
    public void Not_equal_when_field_is_different(BackupSettings backupSettings1, BackupSettings backupSettings2) =>
        Assert.That(backupSettings1, Is.Not.EqualTo(backupSettings2));
    
    private static object[] _notEqualTestCases =
    {
        new BackupSettings[] { new() {FileName = "A"}, new() {FileName = "B"} },
        new BackupSettings[] { new() {AzureStorageAccountName = "A"}, new() {AzureStorageAccountName = "B"} },
        new BackupSettings[] { new() {Container = "A"}, new() {Container = "B"} },
        new BackupSettings[] { new() {TrackedFiles = new List<string>{"A"}}, new() {TrackedFiles = new List<string>{"B"}} },
        new BackupSettings[] { new() {TrackedFolders = new List<string>{"A"}}, new() {TrackedFolders = new List<string>{"B"}} },
    };
    
    [TestCaseSource(nameof(_equalTestCases))]
    public void Equal_when_field_is_same(BackupSettings backupSettings1, BackupSettings backupSettings2) =>
        Assert.That(backupSettings1, Is.EqualTo(backupSettings2));
    
    private static object[] _equalTestCases =
    {
        new BackupSettings[] { new() {FileName = "A"}, new() {FileName = "A"} },
        new BackupSettings[] { new() {FileName = "A"}, new() {FileName = "A", PreviousHashesOfFiles = new List<FileHash>{new("file","hash")}} },
        new BackupSettings[] { new() {AzureStorageAccountName = "A"}, new() {AzureStorageAccountName = "A"} },
        new BackupSettings[] { new() {Container = "A"}, new() {Container = "A"} },
        new BackupSettings[] { new() {TrackedFiles = new List<string>{"A"}}, new() {TrackedFiles = new List<string>{"A"}} },
        new BackupSettings[] { new() {TrackedFolders = new List<string>{"A"}}, new() {TrackedFolders = new List<string>{"A"}} },
    };
    
    [Test]
    public void Return_serialised_string_of_instance_of_backup_settings()
    {
        var backupSettings = new BackupSettings
        {
            FileName = "file",
            AzureStorageAccountName = "account123",
            Container = "backups",
            TrackedFiles = new List<string>{"file1","File2"},
            TrackedFolders = new List<string>{"Folder1", "Folder2"},
            PreviousHashesOfFiles = new List<FileHash>{new("file1", "a"),new("file2","b")}
        };
        var serialisedContent = $"{{\"FileName\":\"{backupSettings.FileName}\",\"AzureStorageAccountName\":\"{backupSettings.AzureStorageAccountName}\",\"Container\":\"{backupSettings.Container}\",\"TrackedFiles\":[\"{string.Join("\",\"", backupSettings.TrackedFiles)}\"],\"TrackedFolders\":[\"{string.Join("\",\"", backupSettings.TrackedFolders)}\"],\"PreviousHashesOfFiles\":[{string.Join(',', backupSettings.PreviousHashesOfFiles.Select(p => $"{{\"FileName\":\"{p.FileName}\",\"Hash\":\"{p.Hash}\"}}"))}]}}";
        
        var returnedContent = backupSettings.ToSerialisedString();
        Assert.That(returnedContent, Is.EqualTo(serialisedContent));
    }
}
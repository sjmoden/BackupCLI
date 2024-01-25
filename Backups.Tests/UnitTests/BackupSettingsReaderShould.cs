using System.IO.Abstractions.TestingHelpers;
using Backups.Actions;
using Backups.Infrastructure;
using Backups.Model;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class BackupSettingsReaderShould
{
    private const string Directory = @"c:\holiday";
    private const string FilePath = $@"{Directory}\.backupSettings";

    [Test]
    public void Throw_file_not_found_exception_when_no_file_exists()
    {
        Assert.Throws<BackupSettingsNotFoundException>(() =>
        {
            var mockFileSystem = new MockFileSystem();
            new BackupSettingsReader(mockFileSystem, new BackupSettingFilePath(mockFileSystem)).Read(@"c:\holiday");
        });
    }

    [Test]
    public void Throw_exception_when_when_file_does_not_deserialise()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { FilePath, new MockFileData("Corrupt data") }
        });
        
        var backupSettingsReader = new BackupSettingsReader(fileSystem, new BackupSettingFilePath(fileSystem));
        Assert.Throws<BackupSettingsDeserialiseException>(() => backupSettingsReader.Read(Directory));
    }
    
    [Test]
    public void Return_empty_backup_settings_when_file_does_not_deserialise()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { FilePath, new MockFileData(string.Empty) }
        });
        
        var backupSettingsReader = new BackupSettingsReader(fileSystem, new BackupSettingFilePath(fileSystem));
        var returnedBackupSetting = backupSettingsReader.Read(Directory);
        Assert.That(returnedBackupSetting.FileName, Is.Null);
    }
    
    [Test]
    public void Return_backup_settings_when_read()
    {
        const string fileName = "holiday pics";
        const string trackedFile1 = "tracked.file";
        const string trackedFile2 = "tracked2.file";
        const string trackedFolder1 = "trackedFolder1";
        const string trackedFolder2 = "trackedFolder2";
        const string storageAccountName = "storage246857";
        const string container = "backUp";
        var fileHash1 = new FileHash("a.File", "a");
        var fileHash2 = new FileHash("b.File", "b");
        var fileContents = $$"""
                                      {
                                          "FileName": "{{fileName}}",
                                          "TrackedFiles": ["{{trackedFile1}}", "{{trackedFile2}}"],
                                          "TrackedFolders": ["{{trackedFolder1}}", "{{trackedFolder2}}"],
                                          "AzureStorageAccountName": "{{storageAccountName}}",
                                          "Container": "{{container}}",
                                          "PreviousHashesOfFiles":[
                                            {"FileName":"{{fileHash1.FileName}}","Hash":"{{fileHash1.Hash}}"},
                                            {"FileName":"{{fileHash2.FileName}}","Hash":"{{fileHash2.Hash}}"}
                                          ]
                                      }
                                      """;

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { FilePath, new MockFileData(fileContents) }
        });
        
        var backupSettingsReader = new BackupSettingsReader(fileSystem, new BackupSettingFilePath(fileSystem));
        var returnedBackupSetting = backupSettingsReader.Read(Directory);
        
        Assert.Multiple(() =>
        {
            Assert.That(returnedBackupSetting.FileName, Is.EqualTo(fileName));    
            Assert.That(returnedBackupSetting.TrackedFiles, Is.EquivalentTo(new List<string>{trackedFile1, trackedFile2}));
            Assert.That(returnedBackupSetting.TrackedFolders, Is.EquivalentTo(new List<string>{trackedFolder1, trackedFolder2}));
            Assert.That(returnedBackupSetting.AzureStorageAccountName, Is.EqualTo(storageAccountName));
            Assert.That(returnedBackupSetting.Container, Is.EqualTo(container));
            Assert.That(returnedBackupSetting.PreviousHashesOfFiles, Is.EquivalentTo(new List<FileHash>{fileHash1,fileHash2}));
        });
        
    }
}
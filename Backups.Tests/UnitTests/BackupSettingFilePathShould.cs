using System.IO.Abstractions.TestingHelpers;
using Backups.Infrastructure;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class BackupSettingFilePathShould
{
    [Test]
    public void Throw_BackupSettingsNotFoundException_when_settings_do_not_exists_in_directory()
    {
        var backupSettingFilePath = new BackupSettingFilePath(new MockFileSystem());
        Assert.Throws<BackupSettingsNotFoundException>(() => backupSettingFilePath.ThrowExceptionIfSettingsFilesDoesNotExist(@"c:\"));
    }

    [Test]
    public void Not_throw_exception_when_file_exists_in_directory()
    {
        const string directory = @"C:\holidays";
        const string settingsFileName = ".backupSettings";
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { { $@"{directory}\{settingsFileName}", new MockFileData(string.Empty) }});
        
        var backupSettingFilePath = new BackupSettingFilePath(mockFileSystem);
        Assert.DoesNotThrow(() => backupSettingFilePath.ThrowExceptionIfSettingsFilesDoesNotExist(directory));
    }
}
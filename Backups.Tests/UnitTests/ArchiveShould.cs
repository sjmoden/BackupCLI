using Backups.Actions;
using Backups.Actions.Archive;
using Backups.Model;
using FakeItEasy;

namespace Backups.Tests.UnitTests;

[TestFixture]
public class ArchiveShould
{
    private class MockBackupSettings : BackupSettings
    {
        public override bool HaveFilesChangedSinceLastUploadAndUpdateRegister()
        {
            return true;
        }
    }
    
    private const string FileName = "file.7z";
    private const string FilePath = $@"c:\temp\{FileName}";
    private IBackupSettingsReader _backupSettingsReader = null!;
    private ITempArchiveZip _zipArchive = null!;
    private BackupSettings _backupSettings = null!;
    private IKeyVaultPasswordRetriever _keyVaultPasswordRetriever = null!;
    private IAzureBlobStorageUploader _azureBlobStorageUploader = null!;
    private string _blobPath = null!;
    private IBackupSettingsModifier _backupSettingsModifier = null!;

    [SetUp]
    public void SetUp()
    {
        _backupSettingsModifier = A.Fake<IBackupSettingsModifier>();
        _backupSettingsReader = A.Fake<IBackupSettingsReader>();
        _zipArchive = A.Fake<ITempArchiveZip>();
        _backupSettings = new MockBackupSettings
        {
            TrackedFiles = new List<string>{"file1.jpg"},
            AzureStorageAccountName = "account54364",
            Container = "backups",
            FileName = "abc"
        };
        A.CallTo(() => _backupSettingsReader.Read(Environment.CurrentDirectory)).Returns(_backupSettings);
        A.CallTo(() => _zipArchive.ZipFileToTempFolder(A<BackupSettings>._, A<string>._))
            .Returns(FilePath);
        _keyVaultPasswordRetriever = A.Fake<IKeyVaultPasswordRetriever>();
        _azureBlobStorageUploader = A.Fake<IAzureBlobStorageUploader>();
        _blobPath = $@"{_backupSettings.FileName}\{FileName}"; 
    }
    
    [Test]
    public void Zip_up_all_files_listed_in_backup_settings_to_temp_folder()
    {
        const string password = "Password";
        A.CallTo(() => _keyVaultPasswordRetriever.GetOrCreateANewPassword(_backupSettings.FileName)).Returns(password);

        var archive = new Archive(_keyVaultPasswordRetriever, _backupSettingsReader, _zipArchive,
            _azureBlobStorageUploader, _backupSettingsModifier);
        archive.Execute();

        A.CallTo(() => _zipArchive.ZipFileToTempFolder(_backupSettings, password)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void Upload_zip_file_to_azure_blob_storage()
    {
        var archive = new Archive(_keyVaultPasswordRetriever, _backupSettingsReader, _zipArchive,
            _azureBlobStorageUploader, _backupSettingsModifier);
        archive.Execute();
        
        A.CallTo(() => _azureBlobStorageUploader.Upload(new UploadParameters(_backupSettings.AzureStorageAccountName, _backupSettings.Container, FilePath, _blobPath))).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void Delete_the_temp_archive()
    {
        var archive = new Archive(_keyVaultPasswordRetriever, _backupSettingsReader, _zipArchive,
            _azureBlobStorageUploader, _backupSettingsModifier);
        archive.Execute();
        
        A.CallTo(() => _zipArchive.DeleteTempZip(FilePath)).MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public void Delete_the_temp_archive_if_upload_errors()
    {
        A.CallTo(() => _azureBlobStorageUploader.Upload(A<UploadParameters>._)).Throws(new Exception());

        var archive = new Archive(_keyVaultPasswordRetriever, _backupSettingsReader, _zipArchive,
            _azureBlobStorageUploader, _backupSettingsModifier);
        try
        {
            archive.Execute();    
        }
        catch
        {
            // ignored
        }

        A.CallTo(() => _zipArchive.DeleteTempZip(FilePath)).MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public void Calls_methods_in_the_correct_order()
    {
        var archive = new Archive(_keyVaultPasswordRetriever, _backupSettingsReader, _zipArchive,
            _azureBlobStorageUploader, _backupSettingsModifier);
        archive.Execute();
        
        A.CallTo(() => _zipArchive.ZipFileToTempFolder(A<BackupSettings>._, A<string>._)).MustHaveHappenedOnceExactly()
            .Then(A.CallTo(() => _azureBlobStorageUploader.Upload(A<UploadParameters>._)).MustHaveHappenedOnceExactly())
            .Then(A.CallTo(() => _backupSettingsModifier.Overwrite(A<string>._, A<BackupSettings>._)).MustHaveHappenedOnceExactly())
            .Then(A.CallTo(() => _zipArchive.DeleteTempZip(A<string>._)).MustHaveHappenedOnceExactly());
    }

    [Test]
    public void Overwrite_backup_settings()
    {
        var archive = new Archive(_keyVaultPasswordRetriever, _backupSettingsReader, _zipArchive, _azureBlobStorageUploader, _backupSettingsModifier);
        archive.Execute();
        
        A.CallTo(() => _backupSettingsModifier.Overwrite(Environment.CurrentDirectory, _backupSettings)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void Not_zip_file_or_upload_when_file_hashes_have_not_changed_since_last_backup()
    {
        _backupSettings = A.Fake<BackupSettings>();
        A.CallTo(() => _backupSettingsReader.Read(Environment.CurrentDirectory)).Returns(_backupSettings);

        A.CallTo(() => _backupSettings.HaveFilesChangedSinceLastUploadAndUpdateRegister()).Returns(false);
        var archive = new Archive(_keyVaultPasswordRetriever, _backupSettingsReader, _zipArchive, _azureBlobStorageUploader, _backupSettingsModifier);
        archive.Execute();

        A.CallTo(() => _zipArchive.ZipFileToTempFolder(A<BackupSettings>._, A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _azureBlobStorageUploader.Upload(A<UploadParameters>._)).MustNotHaveHappened();
    }
}
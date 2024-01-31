namespace Backups.Actions.Archive;

public class Archive
{
    private readonly IKeyVaultPasswordRetriever _keyVaultPasswordRetriever;
    private readonly IBackupSettingsReader _backupSettingsReader;
    private readonly ITempArchiveZip _zip;
    private readonly IAzureBlobStorageUploader _azureBlobStorageUploader;
    private readonly IBackupSettingsModifier _backupSettingsModifier;

    public Archive(
        IKeyVaultPasswordRetriever keyVaultPasswordRetriever,
        IBackupSettingsReader backupSettingsReader,
        ITempArchiveZip zip,
        IAzureBlobStorageUploader azureBlobStorageUploader,
        IBackupSettingsModifier backupSettingsModifier)
    {
        _keyVaultPasswordRetriever = keyVaultPasswordRetriever;
        _backupSettingsReader = backupSettingsReader;
        _zip = zip;
        _azureBlobStorageUploader = azureBlobStorageUploader;
        _backupSettingsModifier = backupSettingsModifier;
    }

    public void Execute()
    {
        var backupSettings = _backupSettingsReader.Read(Environment.CurrentDirectory);
        var password = _keyVaultPasswordRetriever.GetOrCreateANewPassword(backupSettings.FileName);
        if (!backupSettings.HaveFilesChangedSinceLastUploadAndUpdateRegister())
        {
            return;
        }
        var zipFileToTempFolder = _zip.ZipFileToTempFolder(backupSettings, password);
        
        try
        {
            _azureBlobStorageUploader.Upload(new UploadParameters(backupSettings.AzureStorageAccountName,
                backupSettings.Container, zipFileToTempFolder, $@"{backupSettings.FileName}\{Path.GetFileName(zipFileToTempFolder)}"));
            _backupSettingsModifier.Overwrite(Environment.CurrentDirectory, backupSettings);
        }
        finally
        {
           // _zip.DeleteTempZip(zipFileToTempFolder);            
        } 
    }
}
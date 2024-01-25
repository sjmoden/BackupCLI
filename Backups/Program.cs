using System.IO.Abstractions;
using Backups.Actions;
using Backups.Actions.Archive;
using Backups.Actions.Evaluate;
using Backups.Actions.Initialise;
using Backups.Actions.Track;
using Backups.Infrastructure;
using Backups.Model;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PasswordGenerator;

namespace Backups;

internal static class Program
{
    private static void Main(string[] args)
    {
        ServiceCollection services = new ();
        services.AddTransient<IFileSystem, FileSystem>();
        services.AddTransient<IBackupSettingsModifier, BackupSettingsModifier>();
        services.AddTransient<InitialiseFolder>();
        services.AddTransient<BackupSettingFilePath>();
        services.AddTransient<IEvaluateFolder,EvaluateFolder>();
        services.AddTransient<IConsoleWrapper,ConsoleWrapper>();
        services.AddTransient<EvaluateCurrentFolder>();
        services.AddTransient<IBackupSettingsReader, BackupSettingsReader>();
        services.AddTransient<IBackupSettingsTrackFile, BackupSettingsTrackFile>();
        services.AddTransient<IBackupSettingsTrackFolder, BackupSettingsTrackFolder>();
        services.AddTransient<IPasswordGenerator, Actions.Archive.PasswordGenerator>();
        services.AddTransient<IPassword, Password>();
        services.AddTransient<ITempArchiveZip, TempArchiveZip>();
        services.AddTransient<IGuidCreator, GuidCreator>();
        services.AddTransient<ISevenZip, SevenZip>();
        services.AddTransient<Track>();
        services.AddTransient<IAzureBlobStorageUploader,AzureBlobStorageUploader>();
        services.AddTransient<IBlobServiceClientFactory,BlobServiceClientFactory>();
        services.AddTransient<IKeyVaultPasswordRetriever,KeyVaultPasswordRetriever>();
        services.AddTransient<IKeyVault,KeyVault>();
        services.AddTransient<ISecretClientFactory,SecretClientFactory>();
        services.AddTransient<IEnvironmentWrapper,EnvironmentWrapper>();
        services.AddTransient<Archive>();
        var provider = services.BuildServiceProvider();
        
        Parser.Default.ParseArguments<InitialiseOptions, EvaluateOptions,TrackOptions, ArchiveOptions>(args)
            .WithParsed((InitialiseOptions options) =>
                provider.GetService<InitialiseFolder>()!.Initialise(options))
            .WithParsed((EvaluateOptions _) =>
                provider.GetService<EvaluateCurrentFolder>()!.Evaluate())
            .WithParsed((TrackOptions opt) =>
                provider.GetService<Track>()!.Execute(opt))
            .WithParsed((ArchiveOptions _) => 
                provider.GetService<Archive>()!.Execute());
    }
}
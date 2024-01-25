using CommandLine;

namespace Backups.Model;

[Verb("init")]
public class InitialiseOptions
{
    [Option("ZipFileName", Required = true, HelpText = "The name of the zip file that will be created.")]
    public string ZipFileName { get; init; }

    [Option("AzureStorageAccountName", Required = true, HelpText = "The name of the storage account that the files will be archived to.")]
    public string AzureStorageAccountName{ get; init; }

    [Option("Container", Required = true, HelpText = "The name of the container that the files will be archived to.")]
    public string Container{ get; init; }
}
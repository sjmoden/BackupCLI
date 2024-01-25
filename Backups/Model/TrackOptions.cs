using CommandLine;

namespace Backups.Model;

[Verb("track")]
public class TrackOptions
{
    [Option("FileName", SetName = "File",Required = true, HelpText = "The name of the file that will be tracked.")]
    public string FileName { get; set; }
    
    [Option("FolderName", SetName = "Folder",Required = true, HelpText = "The name of the folder that will be tracked.")]
    public string FolderName { get; set; }
}
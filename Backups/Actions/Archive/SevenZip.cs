using System.Diagnostics;
using Backups.Infrastructure;

namespace Backups.Actions.Archive;

public record AddFileToZipParameters(string Password, string ZipFilePath, string FileToAddToArchive);

public record UnzipFileParameters(string Password, string ZipFilePath, string UnzipPath);

public interface ISevenZip
{
    void AddFileToZip(AddFileToZipParameters addFileToZipParameters);
    void UnzipFile(UnzipFileParameters unzipFileParameters);
}

public class SevenZip : ISevenZip{
    public void AddFileToZip(AddFileToZipParameters addFileToZipParameters)
    {
        if (string.IsNullOrWhiteSpace(addFileToZipParameters.Password))
        {
            throw new PasswordEmptyException();
        }
        
        ExecuteSevenZip(
            $"a \"{addFileToZipParameters.ZipFilePath}\" \"{addFileToZipParameters.FileToAddToArchive}\" -p{addFileToZipParameters.Password}");
    }

    public void UnzipFile(UnzipFileParameters unzipFileParameters)
    {
        if (string.IsNullOrWhiteSpace(unzipFileParameters.Password))
        {
            throw new PasswordEmptyException();
        }
        
        try
        {
            ExecuteSevenZip(
                $"e {unzipFileParameters.ZipFilePath} -o{unzipFileParameters.UnzipPath} -p{unzipFileParameters.Password}");
        }
        catch (Exception e ) when (e.Message.Contains("Wrong password?"))
        {
            throw new PasswordDoesNotMatchException();
        }
    }

    private static void ExecuteSevenZip(string arguments)
    {
        Console.WriteLine(arguments);
        //return;
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "7z.exe",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processStartInfo };
        process.Start();
        var err = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (!string.IsNullOrWhiteSpace(err)) throw new Exception(err);
    }
}
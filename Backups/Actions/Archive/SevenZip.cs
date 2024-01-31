using System.Diagnostics;
using Backups.Infrastructure;
using Microsoft.Identity.Client;

namespace Backups.Actions.Archive;

public record AddFileToZipParameters(string Password, string ZipFilePath, string FileToAddToArchive);

public record UnzipFileParameters(string Password, string ZipFilePath, string UnzipPath);

public record TestZipParameters(string Password, string ZipFilePath);


public interface ISevenZip
{
    void AddFileToZip(AddFileToZipParameters addFileToZipParameters);
    void TestZip(TestZipParameters testZipParameters);
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

    public void TestZip(TestZipParameters testZipParameters)
    {
        if (string.IsNullOrWhiteSpace(testZipParameters.Password))
        {
            throw new PasswordEmptyException();
        }

        
        ExecuteSevenZip(
            $"t \"{testZipParameters.ZipFilePath}\" -p{testZipParameters.Password}");
    }
}
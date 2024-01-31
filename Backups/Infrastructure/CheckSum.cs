using System.IO.Abstractions;
using System.Security.Cryptography;

namespace Backups.Infrastructure;

public class CheckSum
{
    private readonly IFileSystem _fileSystem;

    public CheckSum(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public string Get(string filePath)
    {
        Console.WriteLine($"Calculating Checksum for {filePath}");
        using var sha = SHA512.Create();
        using var stream = _fileSystem.File.OpenRead(filePath);
        return Convert.ToBase64String(sha.ComputeHash(stream));
    }
}
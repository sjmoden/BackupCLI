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

    public string Get(string file1Path)
    {
        using var md5 = MD5.Create();
        using var stream = _fileSystem.File.OpenRead(file1Path);
        return BitConverter.ToString(md5.ComputeHash(stream));
    }
}
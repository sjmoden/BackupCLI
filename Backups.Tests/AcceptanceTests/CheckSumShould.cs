using System.IO.Abstractions.TestingHelpers;
using Backups.Infrastructure;

namespace Backups.Tests.AcceptanceTests;

[TestFixture]
public class CheckSumShould
{
    private MockFileSystem _mockFileSystem = null!;
    private string _sameContentsFilePath2 = null!;
    private string _sameContentsFilePath1 = null!;
    private string _differentContentsFilePath = null!;
    private CheckSum _checkSum = null!;

    [SetUp]
    public void SetUp()
    {
        _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        const string xDrive = "x:";
        const string cDrive = "c:";
        const string eDrive = "e:";
        _mockFileSystem.AddDirectory(xDrive);
        _mockFileSystem.AddDirectory(cDrive);
        _mockFileSystem.AddDirectory(eDrive);
        const string sameContents = "same contents";
        const string differentContents = "different contents";
        const string testFileName = "test.file";
        _sameContentsFilePath1 = Path.Combine(cDrive, testFileName);
        _sameContentsFilePath2 = Path.Combine(xDrive, testFileName);
        _differentContentsFilePath = Path.Combine(eDrive, testFileName);
        _mockFileSystem.File.WriteAllText(_sameContentsFilePath1, sameContents);
        _mockFileSystem.File.WriteAllText(_sameContentsFilePath2, sameContents);
        _mockFileSystem.File.WriteAllText(_differentContentsFilePath, differentContents);
        _checkSum = new CheckSum(_mockFileSystem);
    }

    [Test]
    public void Generate_same_checksum_when_files_are_the_same()
    {
        var file1CheckSum = _checkSum.Get(_sameContentsFilePath1);
        var file2CheckSum = _checkSum.Get(_sameContentsFilePath2);
        
        Assert.That(file1CheckSum, Is.EqualTo(file2CheckSum));
    }
    
    [Test]
    public void Generate_different_checksum_when_files_are_the_different()
    {
        var file1CheckSum = _checkSum.Get(_sameContentsFilePath1);
        var file2CheckSum = _checkSum.Get(_differentContentsFilePath);
        
        Assert.That(file1CheckSum, Is.Not.EqualTo(file2CheckSum));
    }
}
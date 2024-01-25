using Backups.Actions.Archive;
using Backups.Infrastructure;

namespace Backups.Tests.IntegrationTests;

[TestFixture]
public class SevenZipShould {
    private const string Password = "TestPassword";

    private string _tempFilePath = null!;
    private string _tempFilePath2 = null!;
    private string _zipPath = null!;
    private string _extractPath = null!;
    private readonly SevenZip _sevenZip = new();

    [SetUp]
    public void SetUp()
    {
        _tempFilePath = Path.GetTempFileName();
        File.WriteAllText(_tempFilePath, "data");
        _tempFilePath2 = Path.GetTempFileName();
        File.WriteAllText(_tempFilePath2, "data");
        _zipPath = Path.Combine(Path.GetTempPath(), "TestFile1.7z");
        _extractPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    }

    [TearDown]
    public void TearDown()
    {
        File.Delete(_tempFilePath);
        File.Delete(_tempFilePath2);

        if (File.Exists(_zipPath))
        {
            File.Delete(_zipPath);
        }
        
        if (File.Exists(_extractPath))
        {
            File.SetAttributes(_extractPath, FileAttributes.Normal);
            File.Delete(_extractPath);
        }
    }

    [Test]
    public void Throw_an_exception_when_trying_to_unzip_a_zip_file_with_the_incorrect_password()
    {
        _sevenZip.AddFileToZip(new AddFileToZipParameters(Password, _zipPath, _tempFilePath));
        Assert.Throws<PasswordDoesNotMatchException>(() =>
            _sevenZip.UnzipFile(new UnzipFileParameters("NotAPassword", _zipPath, _extractPath)));
    }

    [Test]
    public void Throw_an_exception_when_the_password_is_empty_for_unzip()
    {
        Assert.Throws<PasswordEmptyException>(() =>
            _sevenZip.UnzipFile(new UnzipFileParameters("  ", _zipPath, _extractPath)));
    }

    [Test]
    public void Throw_an_exception_when_the_password_is_empty_for_add_file()
    {
        Assert.Throws<PasswordEmptyException>(() =>
            _sevenZip.AddFileToZip(new AddFileToZipParameters("  ", _zipPath, _tempFilePath)));
    }
    
    [Test]
    public void Zip_and_unzip_files()
    {
        var tempFileName = Path.GetFileName(_tempFilePath);
        var tempFileName2 = Path.GetFileName(_tempFilePath2);

        _sevenZip.AddFileToZip(new AddFileToZipParameters(Password, _zipPath, _tempFilePath));
        _sevenZip.AddFileToZip(new AddFileToZipParameters(Password, _zipPath, _tempFilePath2));
        _sevenZip.UnzipFile(new UnzipFileParameters(Password, _zipPath, _extractPath));

        var files = Directory.GetFiles(_extractPath);

        Assert.Multiple(() =>
        {
            Assert.That(files, Has.Length.EqualTo(2));
            Assert.That(files, Has.One.Contain(tempFileName));
            Assert.That(files, Has.One.Contain(tempFileName2));
        });
    }
}
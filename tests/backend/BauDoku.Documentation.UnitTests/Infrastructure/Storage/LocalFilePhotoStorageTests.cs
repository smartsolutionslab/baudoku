using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;
using BauDoku.Documentation.Infrastructure.Storage;
using Microsoft.Extensions.Options;

namespace BauDoku.Documentation.UnitTests.Infrastructure.Storage;

public sealed class LocalFilePhotoStorageTests : IDisposable
{
    private readonly string tempDir;
    private readonly LocalFilePhotoStorage storage;

    public LocalFilePhotoStorageTests()
    {
        tempDir = Path.Combine(Path.GetTempPath(), $"baudoku_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        var options = Options.Create(new PhotoStorageOptions { LocalPath = tempDir });
        storage = new LocalFilePhotoStorage(options);
    }

    [Fact]
    public async Task DownloadAsync_WithValidBlobUrl_ShouldResolvePath()
    {
        var fileName = $"{Guid.NewGuid()}.jpg";
        var filePath = Path.Combine(tempDir, fileName);
        await File.WriteAllTextAsync(filePath, "test content");

        var stream = await storage.DownloadAsync(BlobUrl.From(fileName));
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();

        content.Should().Be("test content");
    }

    [Fact]
    public void DownloadAsync_WithPathTraversal_ShouldThrowUnauthorizedAccessException()
    {
        Func<Task> act = () => storage.DownloadAsync(BlobUrl.From("../../etc/passwd"));
        act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public void DownloadAsync_WithAbsolutePath_ShouldThrowUnauthorizedAccessException()
    {
        Func<Task> act = () => storage.DownloadAsync(BlobUrl.From("/etc/passwd"));
        act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public void DeleteAsync_WithPathTraversal_ShouldThrowUnauthorizedAccessException()
    {
        Func<Task> act = () => storage.DeleteAsync(BlobUrl.From("../../../important.txt"));
        act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task DeleteAsync_WithValidBlobUrl_ShouldDeleteFile()
    {
        var fileName = $"{Guid.NewGuid()}.jpg";
        var filePath = Path.Combine(tempDir, fileName);
        await File.WriteAllTextAsync(filePath, "test content");

        await storage.DeleteAsync(BlobUrl.From(fileName));

        File.Exists(filePath).Should().BeFalse();
    }

    [Fact]
    public async Task UploadAsync_ShouldWriteFileAndReturnBlobUrl()
    {
        var content = "photo bytes"u8.ToArray();
        using var stream = new MemoryStream(content);

        var blobUrl = await storage.UploadAsync(stream, FileName.From("test.jpg"), ContentType.From("image/jpeg"));

        blobUrl.Value.Should().EndWith(".jpg");
        var filePath = Path.Combine(tempDir, blobUrl.Value);
        File.Exists(filePath).Should().BeTrue();
        (await File.ReadAllBytesAsync(filePath)).Should().Equal(content);
    }

    [Fact]
    public void DownloadAsync_WhenMissing_ShouldThrowFileNotFoundException()
    {
        Func<Task> act = () => storage.DownloadAsync(BlobUrl.From("nonexistent.jpg"));

        act.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenMissing_ShouldNotThrow()
    {
        var act = () => storage.DeleteAsync(BlobUrl.From("nonexistent.jpg"));

        await act.Should().NotThrowAsync();
    }

    public void Dispose()
    {
        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, recursive: true);
    }
}

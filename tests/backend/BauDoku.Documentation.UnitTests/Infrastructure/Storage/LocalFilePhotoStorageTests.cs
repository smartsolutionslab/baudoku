using AwesomeAssertions;
using BauDoku.Documentation.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Infrastructure.Storage;

public sealed class LocalFilePhotoStorageTests : IDisposable
{
    private readonly string tempDir;
    private readonly LocalFilePhotoStorage storage;

    public LocalFilePhotoStorageTests()
    {
        tempDir = Path.Combine(Path.GetTempPath(), $"baudoku_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        var configuration = Substitute.For<IConfiguration>();
        configuration["PhotoStorage:LocalPath"].Returns(tempDir);
        storage = new LocalFilePhotoStorage(configuration);
    }

    [Fact]
    public async Task DownloadAsync_WithValidBlobUrl_ShouldResolvePath()
    {
        var fileName = $"{Guid.NewGuid()}.jpg";
        var filePath = Path.Combine(tempDir, fileName);
        await File.WriteAllTextAsync(filePath, "test content");

        var stream = await storage.DownloadAsync(fileName);
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();

        content.Should().Be("test content");
    }

    [Fact]
    public void DownloadAsync_WithPathTraversal_ShouldThrowUnauthorizedAccessException()
    {
        Func<Task> act = () => storage.DownloadAsync("../../etc/passwd");
        act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public void DownloadAsync_WithAbsolutePath_ShouldThrowUnauthorizedAccessException()
    {
        Func<Task> act = () => storage.DownloadAsync("/etc/passwd");
        act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public void DeleteAsync_WithPathTraversal_ShouldThrowUnauthorizedAccessException()
    {
        Func<Task> act = () => storage.DeleteAsync("../../../important.txt");
        act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task DeleteAsync_WithValidBlobUrl_ShouldDeleteFile()
    {
        var fileName = $"{Guid.NewGuid()}.jpg";
        var filePath = Path.Combine(tempDir, fileName);
        await File.WriteAllTextAsync(filePath, "test content");

        await storage.DeleteAsync(fileName);

        File.Exists(filePath).Should().BeFalse();
    }

    public void Dispose()
    {
        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, recursive: true);
    }
}

using AwesomeAssertions;
using BauDoku.BuildingBlocks.Infrastructure.Storage;

namespace BauDoku.BuildingBlocks.UnitTests.Storage;

public sealed class LocalStorageDirectoryTests : IDisposable
{
    private readonly string tempDir;
    private readonly LocalStorageDirectory sut;

    public LocalStorageDirectoryTests()
    {
        tempDir = Path.Combine(Path.GetTempPath(), $"lsd-test-{Guid.NewGuid()}");
        sut = new LocalStorageDirectory(tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, recursive: true);
    }

    [Fact]
    public void Constructor_CreatesDirectory()
    {
        Directory.Exists(tempDir).Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithRelativePath_ResolvesToAbsolute()
    {
        var relativePath = $"relative-test-{Guid.NewGuid()}";
        var storage = new LocalStorageDirectory(relativePath);

        try
        {
            Path.IsPathRooted(storage.BasePath).Should().BeTrue();
            Directory.Exists(storage.BasePath).Should().BeTrue();
        }
        finally
        {
            Directory.Delete(storage.BasePath, recursive: true);
        }
    }

    [Fact]
    public void Resolve_WithValidPath_ReturnsFullPath()
    {
        var result = sut.Resolve("sub/file.txt");

        result.Should().StartWith(Path.GetFullPath(tempDir));
    }

    [Fact]
    public void Resolve_WithTraversalAttempt_ThrowsUnauthorizedAccess()
    {
        var act = () => sut.Resolve("../../../etc/passwd");

        act.Should().Throw<UnauthorizedAccessException>();
    }

    [Fact]
    public void CreateSubdirectory_CreatesAndReturnsPath()
    {
        var result = sut.CreateSubdirectory("child");

        Directory.Exists(result).Should().BeTrue();
        result.Should().StartWith(Path.GetFullPath(tempDir));
    }

    [Fact]
    public void DirectoryExists_WhenExists_ReturnsTrue()
    {
        sut.CreateSubdirectory("existing");

        sut.DirectoryExists("existing").Should().BeTrue();
    }

    [Fact]
    public void DirectoryExists_WhenMissing_ReturnsFalse()
    {
        sut.DirectoryExists("missing").Should().BeFalse();
    }

    [Fact]
    public void DeleteDirectory_WhenExists_RemovesIt()
    {
        sut.CreateSubdirectory("to-delete");

        sut.DeleteDirectory("to-delete");

        sut.DirectoryExists("to-delete").Should().BeFalse();
    }

    [Fact]
    public void DeleteDirectory_WhenMissing_DoesNotThrow()
    {
        var act = () => sut.DeleteDirectory("nonexistent");

        act.Should().NotThrow();
    }

    [Fact]
    public void GetSubdirectories_ReturnsChildDirs()
    {
        sut.CreateSubdirectory("a");
        sut.CreateSubdirectory("b");

        var result = sut.GetSubdirectories();

        result.Should().HaveCount(2);
    }

    [Fact]
    public void GetFiles_ReturnsMatchingFiles()
    {
        sut.CreateSubdirectory("session");
        File.WriteAllText(Path.Combine(sut.Resolve("session"), "0.chunk"), "data");
        File.WriteAllText(Path.Combine(sut.Resolve("session"), "1.chunk"), "data");
        File.WriteAllText(Path.Combine(sut.Resolve("session"), "metadata.json"), "{}");

        var result = sut.GetFiles("session", "*.chunk");

        result.Should().HaveCount(2);
    }

    [Fact]
    public void FileExists_WhenExists_ReturnsTrue()
    {
        File.WriteAllText(Path.Combine(tempDir, "exists.txt"), "content");

        sut.FileExists("exists.txt").Should().BeTrue();
    }

    [Fact]
    public void FileExists_WhenMissing_ReturnsFalse()
    {
        sut.FileExists("missing.txt").Should().BeFalse();
    }

    [Fact]
    public void DeleteFile_WhenExists_RemovesIt()
    {
        File.WriteAllText(Path.Combine(tempDir, "to-delete.txt"), "content");

        sut.DeleteFile("to-delete.txt");

        sut.FileExists("to-delete.txt").Should().BeFalse();
    }

    [Fact]
    public void DeleteFile_WhenMissing_DoesNotThrow()
    {
        var act = () => sut.DeleteFile("nonexistent.txt");

        act.Should().NotThrow();
    }

    [Fact]
    public async Task ReadAllTextAsync_ReturnsFileContent()
    {
        File.WriteAllText(Path.Combine(tempDir, "read.txt"), "hello world");

        var result = await sut.ReadAllTextAsync("read.txt");

        result.Should().Be("hello world");
    }

    [Fact]
    public async Task ReadAllTextAsync_WhenMissing_ThrowsFileNotFound()
    {
        var act = () => sut.ReadAllTextAsync("missing.txt");

        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task WriteAllTextAsync_CreatesFileWithContent()
    {
        await sut.WriteAllTextAsync("write.txt", "written content");

        File.ReadAllText(Path.Combine(tempDir, "write.txt")).Should().Be("written content");
    }

    [Fact]
    public async Task WriteStreamAsync_WritesStreamContent()
    {
        var data = "stream content"u8.ToArray();
        using var stream = new MemoryStream(data);

        await sut.WriteStreamAsync("stream.bin", stream);

        File.ReadAllBytes(Path.Combine(tempDir, "stream.bin")).Should().Equal(data);
    }

    [Fact]
    public void OpenRead_ReturnsReadableStream()
    {
        File.WriteAllText(Path.Combine(tempDir, "readable.txt"), "read me");

        using var stream = sut.OpenRead("readable.txt");

        using var reader = new StreamReader(stream);
        reader.ReadToEnd().Should().Be("read me");
    }

    [Fact]
    public void OpenWrite_CreatesWritableStream()
    {
        using (var stream = sut.OpenWrite("writable.txt"))
        {
            stream.Write("written"u8);
        }

        File.ReadAllText(Path.Combine(tempDir, "writable.txt")).Should().Be("written");
    }

    [Fact]
    public async Task OpenReadAsync_YieldsFileContentInChunks()
    {
        var content = new byte[10_000];
        Random.Shared.NextBytes(content);
        await File.WriteAllBytesAsync(Path.Combine(tempDir, "large.bin"), content);

        var collected = new List<byte>();
        await foreach (var chunk in sut.OpenReadAsync("large.bin", bufferSize: 4096))
        {
            chunk.Length.Should().BeGreaterThan(0);
            chunk.Length.Should().BeLessThanOrEqualTo(4096);
            collected.AddRange(chunk);
        }

        collected.ToArray().Should().Equal(content);
    }

    [Fact]
    public async Task OpenReadAsync_WithSmallFile_YieldsSingleChunk()
    {
        var content = "small"u8.ToArray();
        await File.WriteAllBytesAsync(Path.Combine(tempDir, "small.bin"), content);

        var chunks = new List<byte[]>();
        await foreach (var chunk in sut.OpenReadAsync("small.bin"))
        {
            chunks.Add(chunk);
        }

        chunks.Should().HaveCount(1);
        chunks[0].Should().Equal(content);
    }

    [Fact]
    public async Task OpenReadAsync_WhenMissing_ThrowsFileNotFound()
    {
        var act = async () =>
        {
            await foreach (var _ in sut.OpenReadAsync("missing.bin"))
            {
            }
        };

        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task OpenReadAsync_SupportsCancellation()
    {
        var content = new byte[100_000];
        Random.Shared.NextBytes(content);
        await File.WriteAllBytesAsync(Path.Combine(tempDir, "cancel.bin"), content);

        using var cts = new CancellationTokenSource();
        var chunksRead = 0;

        var act = async () =>
        {
            await foreach (var _ in sut.OpenReadAsync("cancel.bin", bufferSize: 1024, cancellationToken: cts.Token))
            {
                chunksRead++;
                if (chunksRead == 2) await cts.CancelAsync();
            }
        };

        await act.Should().ThrowAsync<OperationCanceledException>();
        chunksRead.Should().Be(2);
    }

    [Fact]
    public async Task OpenReadAsync_WithEmptyFile_YieldsNothing()
    {
        await File.WriteAllBytesAsync(Path.Combine(tempDir, "empty.bin"), []);

        var chunks = new List<byte[]>();
        await foreach (var chunk in sut.OpenReadAsync("empty.bin"))
        {
            chunks.Add(chunk);
        }

        chunks.Should().BeEmpty();
    }

    [Fact]
    public void Resolve_WithTraversalInSubpath_ThrowsUnauthorizedAccess()
    {
        sut.CreateSubdirectory("legit");

        var act = () => sut.Resolve("legit/../../escape");

        act.Should().Throw<UnauthorizedAccessException>();
    }

    [Fact]
    public void EnsureFileExists_WhenExists_DoesNotThrow()
    {
        File.WriteAllText(Path.Combine(tempDir, "present.txt"), "content");

        var act = () => sut.EnsureFileExists("present.txt");

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureFileExists_WhenMissing_ThrowsFileNotFound()
    {
        var act = () => sut.EnsureFileExists("missing.txt");

        act.Should().Throw<FileNotFoundException>()
            .Which.FileName.Should().Be("missing.txt");
    }

    [Fact]
    public void EnsureFileExists_WithTraversalAttempt_ThrowsUnauthorizedAccess()
    {
        var act = () => sut.EnsureFileExists("../../etc/passwd");

        act.Should().Throw<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task WriteJsonAsync_SerializesAndWritesFile()
    {
        var data = new TestData("hello", 42);

        await sut.WriteJsonAsync("data.json", data);

        var json = await File.ReadAllTextAsync(Path.Combine(tempDir, "data.json"));
        json.Should().Contain("\"Name\":\"hello\"");
        json.Should().Contain("\"Count\":42");
    }

    [Fact]
    public async Task ReadJsonAsync_DeserializesFileContent()
    {
        await File.WriteAllTextAsync(Path.Combine(tempDir, "data.json"), """{"Name":"world","Count":7}""");

        var result = await sut.ReadJsonAsync<TestData>("data.json");

        result.Should().NotBeNull();
        result!.Name.Should().Be("world");
        result.Count.Should().Be(7);
    }

    [Fact]
    public async Task ReadJsonAsync_WhenMissing_ThrowsFileNotFound()
    {
        var act = () => sut.ReadJsonAsync<TestData>("missing.json");

        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task WriteJsonAsync_ThenReadJsonAsync_RoundTrip()
    {
        var original = new TestData("roundtrip", 99);

        await sut.WriteJsonAsync("roundtrip.json", original);
        var result = await sut.ReadJsonAsync<TestData>("roundtrip.json");

        result.Should().Be(original);
    }

    private sealed record TestData(string Name, int Count);
}

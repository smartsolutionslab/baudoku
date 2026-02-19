using System.Runtime.CompilerServices;
using System.Text.Json;

namespace BauDoku.BuildingBlocks.Infrastructure.Storage;

public sealed class LocalStorageDirectory
{
    public string BasePath { get; }

    public LocalStorageDirectory(string path)
    {
        BasePath = Path.IsPathRooted(path) ? path : Path.Combine(Directory.GetCurrentDirectory(), path);

        Directory.CreateDirectory(BasePath);
    }

    public string Resolve(string relativePath)
    {
        var combined = Path.Combine(BasePath, relativePath);
        var fullPath = Path.GetFullPath(combined);

        return !fullPath.StartsWith(Path.GetFullPath(BasePath), StringComparison.OrdinalIgnoreCase) ? throw new UnauthorizedAccessException($"Zugriff verweigert: {relativePath}") : fullPath;
    }

    public string CreateSubdirectory(string relativePath)
    {
        var fullPath = Resolve(relativePath);
        Directory.CreateDirectory(fullPath);
        return fullPath;
    }

    public bool DirectoryExists(string relativePath) => Directory.Exists(Resolve(relativePath));

    public void DeleteDirectory(string relativePath)
    {
        var fullPath = Resolve(relativePath);
        if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, recursive: true);
        }
    }

    public string[] GetSubdirectories() => Directory.Exists(BasePath) ? Directory.GetDirectories(BasePath) : [];

    public string[] GetFiles(string relativePath, string pattern) => Directory.GetFiles(Resolve(relativePath), pattern);

    public bool FileExists(string relativePath) => File.Exists(Resolve(relativePath));

    public void EnsureFileExists(string relativePath)
    {
        var fullPath = Resolve(relativePath);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Datei nicht gefunden: {relativePath}", relativePath);
    }

    public void DeleteFile(string relativePath)
    {
        var fullPath = Resolve(relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    public async Task<string> ReadAllTextAsync(string relativePath, CancellationToken cancellationToken = default)
        => await File.ReadAllTextAsync(Resolve(relativePath), cancellationToken);

    public async Task WriteAllTextAsync(string relativePath, string content, CancellationToken cancellationToken = default)
        => await File.WriteAllTextAsync(Resolve(relativePath), content, cancellationToken);

    public async Task<T?> ReadJsonAsync<T>(string relativePath, CancellationToken ct = default)
    {
        var json = await ReadAllTextAsync(relativePath, ct);
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task WriteJsonAsync<T>(string relativePath, T value, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value);
        await WriteAllTextAsync(relativePath, json, ct);
    }

    public async Task WriteStreamAsync(string relativePath, Stream data, CancellationToken ct = default)
    {
        var fullPath = Resolve(relativePath);
        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await data.CopyToAsync(fileStream, ct);
    }

    public FileStream OpenRead(string relativePath) => new(Resolve(relativePath), FileMode.Open, FileAccess.Read);

    public async IAsyncEnumerable<byte[]> OpenReadAsync(string relativePath, int bufferSize = 4096, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        EnsureFileExists(relativePath);

        await using var stream = new FileStream(Resolve(relativePath), FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
        var buffer = new byte[bufferSize];
        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, bufferSize), cancellationToken)) > 0)
        {
            yield return buffer[..bytesRead];
        }
    }

    public FileStream OpenWrite(string relativePath) => new(Resolve(relativePath), FileMode.Create, FileAccess.Write);
}

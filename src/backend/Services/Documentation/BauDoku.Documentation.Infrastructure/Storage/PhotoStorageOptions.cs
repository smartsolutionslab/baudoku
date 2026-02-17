namespace BauDoku.Documentation.Infrastructure.Storage;

public sealed class PhotoStorageOptions
{
    public string Provider { get; set; } = "Local";
    public string LocalPath { get; set; } = "uploads/photos";
    public string ChunkedPath { get; set; } = "uploads/chunks";
    public string? AzureConnectionString { get; set; }
    public string AzureContainerName { get; set; } = "photos";
}

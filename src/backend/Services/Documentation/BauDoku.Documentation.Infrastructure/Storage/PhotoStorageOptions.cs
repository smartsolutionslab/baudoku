namespace SmartSolutionsLab.BauDoku.Documentation.Infrastructure.Storage;

public sealed class PhotoStorageOptions
{
    public const string SectionName = "PhotoStorage";
    public const string ProviderLocal = "Local";
    public const string ProviderAzure = "Azure";

    public string Provider { get; set; } = ProviderLocal;
    public string LocalPath { get; set; } = "uploads/photos";
    public string ChunkedPath { get; set; } = "uploads/chunks";
    public string? AzureConnectionString { get; set; }
    public string AzureContainerName { get; set; } = "photos";

    public bool IsAzure => Provider.Equals(ProviderAzure, StringComparison.OrdinalIgnoreCase);
}

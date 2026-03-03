namespace BauDoku.ResourceService;

public sealed class KubernetesOptions
{
    public const string SectionName = "Kubernetes";

    public string Namespace { get; set; } = "default";
    public string? LabelSelector { get; set; }
    public string ApplicationName { get; set; } = "BauDoku";
    public int ReconnectDelaySeconds { get; set; } = 5;
    public int LogTailLines { get; set; } = 1000;
}

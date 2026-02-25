using k8s;

namespace BauDoku.ResourceService.Services;

public sealed class PodLogStreamer
{
    private readonly IKubernetes kubernetes;
    private readonly ILogger<PodLogStreamer> logger;
    private readonly string kubernetesNamespace;
    private readonly int tailLines;

    public PodLogStreamer(IKubernetes kubernetes, IConfiguration configuration, ILogger<PodLogStreamer> logger)
    {
        this.kubernetes = kubernetes;
        this.logger = logger;
        kubernetesNamespace = configuration["KUBERNETES_NAMESPACE"] ?? "default";
        tailLines = configuration.GetValue<int>("PodLogs:TailLines", 1000);
    }

    public async IAsyncEnumerable<(string Line, bool IsStdErr)> StreamLogsAsync(
        string podName,
        bool follow,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting log stream for pod {Pod} (follow={Follow})", podName, follow);

        Stream stream;
        try
        {
            stream = await kubernetes.CoreV1.ReadNamespacedPodLogAsync(
                podName,
                kubernetesNamespace,
                follow: follow,
                tailLines: tailLines,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to open log stream for pod {Pod}", podName);
            yield break;
        }

        await using (stream)
        using (var reader = new StreamReader(stream))
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string? line;
                try
                {
                    line = await reader.ReadLineAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Log stream read error for pod {Pod}", podName);
                    break;
                }

                if (line is null)
                    break;

                yield return (line, false);
            }
        }

        logger.LogInformation("Log stream ended for pod {Pod}", podName);
    }
}

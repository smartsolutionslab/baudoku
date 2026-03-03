using k8s;
using Microsoft.Extensions.Options;

namespace SmartSolutionsLab.BauDoku.ResourceService.Services;

public sealed class PodLogStreamer(IKubernetes kubernetes, IOptions<KubernetesOptions> options, ILogger<PodLogStreamer> logger)
{
    private readonly KubernetesOptions config = options.Value;

    public async IAsyncEnumerable<(string Line, bool IsStdErr)> StreamLogsAsync(string podName, bool follow, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting log stream for pod {Pod} (follow={Follow})", podName, follow);

        Stream stream;
        try
        {
            stream = await kubernetes.CoreV1.ReadNamespacedPodLogAsync(
                podName,
                config.Namespace,
                follow: follow,
                tailLines: config.LogTailLines,
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

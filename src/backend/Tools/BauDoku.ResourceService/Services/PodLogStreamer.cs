using k8s;
using Microsoft.Extensions.Options;

namespace SmartSolutionsLab.BauDoku.ResourceService.Services;

public sealed partial class PodLogStreamer(IKubernetes kubernetes, IOptions<KubernetesOptions> options, ILogger<PodLogStreamer> logger)
{
    private readonly KubernetesOptions config = options.Value;

    public async IAsyncEnumerable<(string Line, bool IsStdErr)> StreamLogsAsync(string podName, bool follow, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        LogStreamStarted(podName, follow);

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
            LogStreamOpenFailed(ex, podName);
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
                    LogStreamReadError(ex, podName);
                    break;
                }

                if (line is null)
                    break;

                yield return (line, false);
            }
        }

        LogStreamEnded(podName);
    }

    [LoggerMessage(EventId = 8010, Level = LogLevel.Information,
        Message = "Starting log stream for pod {Pod} (follow={Follow})")]
    private partial void LogStreamStarted(string pod, bool follow);

    [LoggerMessage(EventId = 8011, Level = LogLevel.Error,
        Message = "Failed to open log stream for pod {Pod}")]
    private partial void LogStreamOpenFailed(Exception exception, string pod);

    [LoggerMessage(EventId = 8012, Level = LogLevel.Warning,
        Message = "Log stream read error for pod {Pod}")]
    private partial void LogStreamReadError(Exception exception, string pod);

    [LoggerMessage(EventId = 8013, Level = LogLevel.Information,
        Message = "Log stream ended for pod {Pod}")]
    private partial void LogStreamEnded(string pod);
}

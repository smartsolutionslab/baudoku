using System.Collections.Concurrent;
using System.Threading.Channels;
using Aspire.DashboardService.Proto.V1;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Options;

namespace SmartSolutionsLab.BauDoku.ResourceService.Services;

public sealed class PodWatcher(IKubernetes kubernetes, IOptions<KubernetesOptions> options, ILogger<PodWatcher> logger)
    : BackgroundService
{
    private readonly KubernetesOptions config = options.Value;
    private readonly ConcurrentDictionary<string, Resource> resources = new();
    private readonly List<Channel<WatchResourcesUpdate>> subscribers = [];
    private readonly Lock subscriberLock = new();

    public IReadOnlyDictionary<string, Resource> CurrentResources => resources;

    public Channel<WatchResourcesUpdate> Subscribe()
    {
        var channel = Channel.CreateUnbounded<WatchResourcesUpdate>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        lock (subscriberLock)
        {
            subscribers.Add(channel);
        }

        return channel;
    }

    public void Unsubscribe(Channel<WatchResourcesUpdate> channel)
    {
        lock (subscriberLock)
        {
            subscribers.Remove(channel);
        }
        channel.Writer.TryComplete();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting pod watcher for namespace {Namespace}", config.Namespace);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await WatchPodsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                var reconnectDelay = TimeSpan.FromSeconds(config.ReconnectDelaySeconds);
                logger.LogError(ex, "Pod watch stream disconnected, reconnecting in {Delay}s", config.ReconnectDelaySeconds);

                await Task.Delay(reconnectDelay, stoppingToken);
            }
        }
    }

    private async Task WatchPodsAsync(CancellationToken stoppingToken)
    {
        var podListResponse = kubernetes.CoreV1.ListNamespacedPodWithHttpMessagesAsync(
            config.Namespace,
            labelSelector: config.LabelSelector,
            watch: true,
            cancellationToken: stoppingToken);

#pragma warning disable CS0618 // WatchAsync overload deprecated but replacement has same name
        await foreach (var (type, pod) in podListResponse.WatchAsync<V1Pod, V1PodList>(cancellationToken: stoppingToken))
#pragma warning restore CS0618
        {
            if (pod.Metadata?.Name is null)
                continue;

            var podName = pod.Metadata.Name;

            switch (type)
            {
                case WatchEventType.Added:
                case WatchEventType.Modified:
                    var resource = ResourceStateMapper.MapPodToResource(pod);
                    resources[podName] = resource;
                    await BroadcastChangeAsync(new WatchResourcesUpdate
                    {
                        Changes = new WatchResourcesChanges
                        {
                            Value =
                            {
                                new WatchResourcesChange { Upsert = resource }
                            }
                        }
                    });
                    logger.LogDebug("Pod {Pod} {EventType}: {State}", podName, type, resource.State);
                    break;

                case WatchEventType.Deleted:
                    resources.TryRemove(podName, out _);
                    await BroadcastChangeAsync(new WatchResourcesUpdate
                    {
                        Changes = new WatchResourcesChanges
                        {
                            Value =
                            {
                                new WatchResourcesChange
                                {
                                    Delete = new ResourceDeletion
                                    {
                                        ResourceName = podName,
                                        ResourceType = "Pod"
                                    }
                                }
                            }
                        }
                    });
                    logger.LogDebug("Pod {Pod} deleted", podName);
                    break;
            }
        }
    }

    private async Task BroadcastChangeAsync(WatchResourcesUpdate update)
    {
        List<Channel<WatchResourcesUpdate>> currentSubscribers;
        lock (subscriberLock)
        {
            currentSubscribers = [.. subscribers];
        }

        foreach (var channel in currentSubscribers)
        {
            try
            {
                await channel.Writer.WriteAsync(update);
            }
            catch
            {
                Unsubscribe(channel);
            }
        }
    }
}

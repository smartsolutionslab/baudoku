using Aspire.DashboardService.Proto.V1;
using Grpc.Core;
using k8s;

namespace BauDoku.ResourceService.Services;

public sealed class KubernetesDashboardService : DashboardService.DashboardServiceBase
{
    private readonly PodWatcher podWatcher;
    private readonly PodLogStreamer podLogStreamer;
    private readonly IKubernetes kubernetes;
    private readonly ILogger<KubernetesDashboardService> logger;
    private readonly string kubernetesNamespace;

    public KubernetesDashboardService(
        PodWatcher podWatcher,
        PodLogStreamer podLogStreamer,
        IKubernetes kubernetes,
        IConfiguration configuration,
        ILogger<KubernetesDashboardService> logger)
    {
        this.podWatcher = podWatcher;
        this.podLogStreamer = podLogStreamer;
        this.kubernetes = kubernetes;
        this.logger = logger;
        kubernetesNamespace = configuration["KUBERNETES_NAMESPACE"] ?? "default";
    }

    public override Task<ApplicationInformationResponse> GetApplicationInformation(
        ApplicationInformationRequest request,
        ServerCallContext context)
    {
        var appName = Environment.GetEnvironmentVariable("APPLICATION_NAME")
            ?? $"BauDoku ({kubernetesNamespace})";

        return Task.FromResult(new ApplicationInformationResponse { ApplicationName = appName });
    }

    public override async Task WatchResources(
        WatchResourcesRequest request,
        IServerStreamWriter<WatchResourcesUpdate> responseStream,
        ServerCallContext context)
    {
        logger.LogInformation("Dashboard connected to WatchResources (reconnect={IsReconnect})", request.IsReconnect);

        // Send initial snapshot
        var initialData = new InitialResourceData();
        initialData.ResourceTypes.Add(new ResourceType { UniqueName = "Deployment", DisplayName = "Deployment" });
        initialData.ResourceTypes.Add(new ResourceType { UniqueName = "StatefulSet", DisplayName = "StatefulSet" });
        initialData.ResourceTypes.Add(new ResourceType { UniqueName = "ReplicaSet", DisplayName = "ReplicaSet" });
        initialData.ResourceTypes.Add(new ResourceType { UniqueName = "Pod", DisplayName = "Pod" });
        initialData.ResourceTypes.Add(new ResourceType { UniqueName = "Job", DisplayName = "Job" });

        foreach (var resource in podWatcher.CurrentResources.Values)
        {
            initialData.Resources.Add(resource);
        }

        await responseStream.WriteAsync(new WatchResourcesUpdate { InitialData = initialData });

        // Stream updates
        var channel = podWatcher.Subscribe();
        try
        {
            await foreach (var update in channel.Reader.ReadAllAsync(context.CancellationToken))
            {
                await responseStream.WriteAsync(update);
            }
        }
        finally
        {
            podWatcher.Unsubscribe(channel);
        }
    }

    public override async Task WatchResourceConsoleLogs(
        WatchResourceConsoleLogsRequest request,
        IServerStreamWriter<WatchResourceConsoleLogsUpdate> responseStream,
        ServerCallContext context)
    {
        logger.LogInformation("Dashboard requested console logs for {Resource}", request.ResourceName);

        var lineNumber = 1;
        var follow = !request.SuppressFollow;

        await foreach (var (line, isStdErr) in podLogStreamer.StreamLogsAsync(
            request.ResourceName,
            follow,
            context.CancellationToken))
        {
            var update = new WatchResourceConsoleLogsUpdate();
            update.LogLines.Add(new ConsoleLogLine
            {
                Text = line,
                IsStdErr = isStdErr,
                LineNumber = lineNumber++
            });

            await responseStream.WriteAsync(update);
        }
    }

    public override async Task<ResourceCommandResponse> ExecuteResourceCommand(
        ResourceCommandRequest request,
        ServerCallContext context)
    {
        logger.LogInformation("Executing command {Command} on {Resource}", request.CommandName, request.ResourceName);

        if (request.CommandName == "restart")
        {
            try
            {
                await kubernetes.CoreV1.DeleteNamespacedPodAsync(
                    request.ResourceName,
                    kubernetesNamespace,
                    cancellationToken: context.CancellationToken);

                return new ResourceCommandResponse
                {
                    Kind = ResourceCommandResponseKind.Succeeded
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to restart pod {Pod}", request.ResourceName);
                return new ResourceCommandResponse
                {
                    Kind = ResourceCommandResponseKind.Failed,
                    ErrorMessage = ex.Message
                };
            }
        }

        return new ResourceCommandResponse
        {
            Kind = ResourceCommandResponseKind.Failed,
            ErrorMessage = $"Unknown command: {request.CommandName}"
        };
    }

    public override async Task WatchInteractions(
        IAsyncStreamReader<WatchInteractionsRequestUpdate> requestStream,
        IServerStreamWriter<WatchInteractionsResponseUpdate> responseStream,
        ServerCallContext context)
    {
        // No-op: interactions not needed for K8s resource service
        await Task.Delay(Timeout.Infinite, context.CancellationToken);
    }
}

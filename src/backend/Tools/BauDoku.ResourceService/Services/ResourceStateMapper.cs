using k8s.Models;
using Aspire.DashboardService.Proto.V1;
using Google.Protobuf.WellKnownTypes;

namespace BauDoku.ResourceService.Services;

public static class ResourceStateMapper
{
    public static (string State, string StateStyle) MapPodState(V1Pod pod)
    {
        var phase = pod.Status?.Phase;

        // Check container statuses for more specific states
        var containerStatuses = pod.Status?.ContainerStatuses;
        if (containerStatuses is { Count: > 0 })
        {
            foreach (var cs in containerStatuses)
            {
                if (cs.State?.Waiting is { } waiting)
                {
                    return waiting.Reason switch
                    {
                        "CrashLoopBackOff" => ("CrashLoopBackOff", "error"),
                        "ImagePullBackOff" => ("ImagePullBackOff", "error"),
                        "ErrImagePull" => ("ImagePullBackOff", "error"),
                        "ContainerCreating" => ("Starting", "info"),
                        "PodInitializing" => ("Starting", "info"),
                        _ => (waiting.Reason ?? "Waiting", "info")
                    };
                }

                if (cs.State?.Terminated is { } terminated)
                {
                    return terminated.Reason switch
                    {
                        "OOMKilled" => ("OOMKilled", "error"),
                        "Error" => ("Failed", "error"),
                        "Completed" => ("Finished", "success"),
                        _ => (terminated.Reason ?? "Terminated", "warning")
                    };
                }
            }

            // All containers running — check readiness
            var allReady = containerStatuses.All(cs => cs.Ready);
            if (phase == "Running" && allReady)
                return ("Running", "success");
            if (phase == "Running" && !allReady)
                return ("Starting", "info");
        }

        return phase switch
        {
            "Pending" => ("Pending", "info"),
            "Running" => ("Running", "success"),
            "Succeeded" => ("Finished", "success"),
            "Failed" => ("Failed", "error"),
            _ => (phase ?? "Unknown", "warning")
        };
    }

    public static Resource MapPodToResource(V1Pod pod)
    {
        var (state, stateStyle) = MapPodState(pod);
        var ownerKind = pod.Metadata?.OwnerReferences?.FirstOrDefault()?.Kind ?? "Pod";

        var resource = new Resource
        {
            Name = pod.Metadata?.Name ?? "unknown",
            ResourceType = ownerKind,
            DisplayName = pod.Metadata?.Name ?? "unknown",
            Uid = pod.Metadata?.Uid ?? System.Guid.NewGuid().ToString(),
            State = state,
            StateStyle = stateStyle
        };

        if (pod.Metadata?.CreationTimestamp is { } created)
            resource.CreatedAt = Timestamp.FromDateTime(created.ToUniversalTime());

        var containerStatus = pod.Status?.ContainerStatuses?.FirstOrDefault();
        if (containerStatus?.State?.Running?.StartedAt is { } started)
            resource.StartedAt = Timestamp.FromDateTime(started.ToUniversalTime());

        // Properties
        var image = pod.Spec?.Containers?.FirstOrDefault()?.Image;
        if (image is not null)
        {
            resource.Properties.Add(new ResourceProperty
            {
                Name = "container.image",
                DisplayName = "Image",
                Value = Google.Protobuf.WellKnownTypes.Value.ForString(image)
            });
        }

        if (pod.Status?.PodIP is { } podIp)
        {
            resource.Properties.Add(new ResourceProperty
            {
                Name = "pod.ip",
                DisplayName = "Pod IP",
                Value = Google.Protobuf.WellKnownTypes.Value.ForString(podIp)
            });
        }

        if (pod.Spec?.NodeName is { } nodeName)
        {
            resource.Properties.Add(new ResourceProperty
            {
                Name = "pod.node",
                DisplayName = "Node",
                Value = Google.Protobuf.WellKnownTypes.Value.ForString(nodeName)
            });
        }

        if (containerStatus is not null)
        {
            resource.Properties.Add(new ResourceProperty
            {
                Name = "container.restarts",
                DisplayName = "Restarts",
                Value = Google.Protobuf.WellKnownTypes.Value.ForNumber(containerStatus.RestartCount)
            });
        }

        // Health reports from container ready conditions
        var conditions = pod.Status?.Conditions;
        if (conditions is not null)
        {
            foreach (var condition in conditions)
            {
                var healthStatus = condition.Status == "True"
                    ? HealthStatus.Healthy
                    : HealthStatus.Unhealthy;

                resource.HealthReports.Add(new HealthReport
                {
                    Key = condition.Type,
                    Status = healthStatus,
                    Description = condition.Message ?? ""
                });
            }
        }

        // Restart command
        resource.Commands.Add(new ResourceCommand
        {
            Name = "restart",
            DisplayName = "Restart",
            ConfirmationMessage = $"Are you sure you want to restart pod '{resource.Name}'?",
            State = ResourceCommandState.Enabled,
            IconName = "ArrowCounterclockwise"
        });

        return resource;
    }
}

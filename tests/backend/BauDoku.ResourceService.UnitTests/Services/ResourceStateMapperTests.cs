using AwesomeAssertions;
using BauDoku.ResourceService.Services;
using k8s.Models;

namespace BauDoku.ResourceService.UnitTests.Services;

public sealed class ResourceStateMapperTests
{
    // ──────────────────────────────────────────────
    //  MapPodState — Waiting states
    // ──────────────────────────────────────────────

    [Theory]
    [InlineData("CrashLoopBackOff", "CrashLoopBackOff", "error")]
    [InlineData("ImagePullBackOff", "ImagePullBackOff", "error")]
    [InlineData("ErrImagePull", "ImagePullBackOff", "error")]
    [InlineData("ContainerCreating", "Starting", "info")]
    [InlineData("PodInitializing", "Starting", "info")]
    public void MapPodState_WaitingWithKnownReason_ReturnsExpected(string reason, string expectedState, string expectedStyle)
    {
        var pod = CreatePodWithWaitingContainer(reason);

        var (state, stateStyle) = ResourceStateMapper.MapPodState(pod);

        state.Should().Be(expectedState);
        stateStyle.Should().Be(expectedStyle);
    }

    [Fact]
    public void MapPodState_WaitingWithUnknownReason_ReturnsReasonAsState()
    {
        var pod = CreatePodWithWaitingContainer("SomeCustomReason");

        var (state, stateStyle) = ResourceStateMapper.MapPodState(pod);

        state.Should().Be("SomeCustomReason");
        stateStyle.Should().Be("info");
    }

    [Fact]
    public void MapPodState_WaitingWithNullReason_ReturnsWaiting()
    {
        var pod = CreatePodWithWaitingContainer(null);

        var (state, stateStyle) = ResourceStateMapper.MapPodState(pod);

        state.Should().Be("Waiting");
        stateStyle.Should().Be("info");
    }

    // ──────────────────────────────────────────────
    //  MapPodState — Terminated states
    // ──────────────────────────────────────────────

    [Theory]
    [InlineData("OOMKilled", "OOMKilled", "error")]
    [InlineData("Error", "Failed", "error")]
    [InlineData("Completed", "Finished", "success")]
    public void MapPodState_TerminatedWithKnownReason_ReturnsExpected(string reason, string expectedState, string expectedStyle)
    {
        var pod = CreatePodWithTerminatedContainer(reason);

        var (state, stateStyle) = ResourceStateMapper.MapPodState(pod);

        state.Should().Be(expectedState);
        stateStyle.Should().Be(expectedStyle);
    }

    [Fact]
    public void MapPodState_TerminatedWithUnknownReason_ReturnsWarning()
    {
        var pod = CreatePodWithTerminatedContainer("SomeOtherReason");

        var (state, stateStyle) = ResourceStateMapper.MapPodState(pod);

        state.Should().Be("SomeOtherReason");
        stateStyle.Should().Be("warning");
    }

    [Fact]
    public void MapPodState_TerminatedWithNullReason_ReturnsTerminatedWarning()
    {
        var pod = CreatePodWithTerminatedContainer(null);

        var (state, stateStyle) = ResourceStateMapper.MapPodState(pod);

        state.Should().Be("Terminated");
        stateStyle.Should().Be("warning");
    }

    // ──────────────────────────────────────────────
    //  MapPodState — Running containers
    // ──────────────────────────────────────────────

    [Fact]
    public void MapPodState_AllContainersReady_ReturnsRunningSuccess()
    {
        var pod = new V1Pod
        {
            Status = new V1PodStatus
            {
                Phase = "Running",
                ContainerStatuses = [CreateContainerStatus(ready: true, running: true)]
            }
        };

        var (state, stateStyle) = ResourceStateMapper.MapPodState(pod);

        state.Should().Be("Running");
        stateStyle.Should().Be("success");
    }

    [Fact]
    public void MapPodState_NotAllContainersReady_ReturnsStartingInfo()
    {
        var pod = new V1Pod
        {
            Status = new V1PodStatus
            {
                Phase = "Running",
                ContainerStatuses = [CreateContainerStatus(ready: false, running: true)]
            }
        };

        var (state, stateStyle) = ResourceStateMapper.MapPodState(pod);

        state.Should().Be("Starting");
        stateStyle.Should().Be("info");
    }

    // ──────────────────────────────────────────────
    //  MapPodState — Phase fallback
    // ──────────────────────────────────────────────

    [Theory]
    [InlineData("Pending", "Pending", "info")]
    [InlineData("Running", "Running", "success")]
    [InlineData("Succeeded", "Finished", "success")]
    [InlineData("Failed", "Failed", "error")]
    public void MapPodState_PhaseFallback_ReturnsExpected(string phase, string expectedState, string expectedStyle)
    {
        var pod = new V1Pod { Status = new V1PodStatus { Phase = phase } };

        var (state, stateStyle) = ResourceStateMapper.MapPodState(pod);

        state.Should().Be(expectedState);
        stateStyle.Should().Be(expectedStyle);
    }

    [Fact]
    public void MapPodState_UnknownPhase_ReturnsWarning()
    {
        var pod = new V1Pod { Status = new V1PodStatus { Phase = "SomethingElse" } };

        var (state, stateStyle) = ResourceStateMapper.MapPodState(pod);

        state.Should().Be("SomethingElse");
        stateStyle.Should().Be("warning");
    }

    [Fact]
    public void MapPodState_NullStatus_ReturnsUnknownWarning()
    {
        var pod = new V1Pod { Status = null };

        var (state, stateStyle) = ResourceStateMapper.MapPodState(pod);

        state.Should().Be("Unknown");
        stateStyle.Should().Be("warning");
    }

    [Fact]
    public void MapPodState_EmptyContainerStatuses_UsesPhase()
    {
        var pod = new V1Pod
        {
            Status = new V1PodStatus
            {
                Phase = "Pending",
                ContainerStatuses = []
            }
        };

        var (state, stateStyle) = ResourceStateMapper.MapPodState(pod);

        state.Should().Be("Pending");
        stateStyle.Should().Be("info");
    }

    // ──────────────────────────────────────────────
    //  MapPodToResource — Name / DisplayName
    // ──────────────────────────────────────────────

    [Fact]
    public void MapPodToResource_SetsNameAndDisplayNameFromMetadata()
    {
        var pod = CreateMinimalPod("my-pod");

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.Name.Should().Be("my-pod");
        resource.DisplayName.Should().Be("my-pod");
    }

    [Fact]
    public void MapPodToResource_NullMetadataName_DefaultsToUnknown()
    {
        var pod = new V1Pod { Metadata = new V1ObjectMeta { Name = null }, Status = new V1PodStatus { Phase = "Running" } };

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.Name.Should().Be("unknown");
        resource.DisplayName.Should().Be("unknown");
    }

    // ──────────────────────────────────────────────
    //  MapPodToResource — ResourceType from ownerReference
    // ──────────────────────────────────────────────

    [Fact]
    public void MapPodToResource_WithOwnerReference_SetsResourceType()
    {
        var pod = CreateMinimalPod("my-pod");
        pod.Metadata!.OwnerReferences = [new V1OwnerReference { Kind = "ReplicaSet" }];

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.ResourceType.Should().Be("ReplicaSet");
    }

    [Fact]
    public void MapPodToResource_NoOwnerReference_DefaultsToPod()
    {
        var pod = CreateMinimalPod("my-pod");

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.ResourceType.Should().Be("Pod");
    }

    // ──────────────────────────────────────────────
    //  MapPodToResource — Uid
    // ──────────────────────────────────────────────

    [Fact]
    public void MapPodToResource_SetsUidFromMetadata()
    {
        var pod = CreateMinimalPod("my-pod");
        pod.Metadata!.Uid = "abc-123";

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.Uid.Should().Be("abc-123");
    }

    [Fact]
    public void MapPodToResource_NullUid_GeneratesGuid()
    {
        var pod = CreateMinimalPod("my-pod");
        pod.Metadata!.Uid = null;

        var resource = ResourceStateMapper.MapPodToResource(pod);

        Guid.TryParse(resource.Uid, out _).Should().BeTrue();
    }

    // ──────────────────────────────────────────────
    //  MapPodToResource — State delegation
    // ──────────────────────────────────────────────

    [Fact]
    public void MapPodToResource_DelegatesStateToMapPodState()
    {
        var pod = CreateMinimalPod("my-pod");
        pod.Status = new V1PodStatus { Phase = "Failed" };

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.State.Should().Be("Failed");
        resource.StateStyle.Should().Be("error");
    }

    // ──────────────────────────────────────────────
    //  MapPodToResource — Timestamps
    // ──────────────────────────────────────────────

    [Fact]
    public void MapPodToResource_WithCreationTimestamp_SetsCreatedAt()
    {
        var pod = CreateMinimalPod("my-pod");
        var now = DateTime.UtcNow;
        pod.Metadata!.CreationTimestamp = now;

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.CreatedAt.Should().NotBeNull();
        resource.CreatedAt.ToDateTime().Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void MapPodToResource_NullCreationTimestamp_OmitsCreatedAt()
    {
        var pod = CreateMinimalPod("my-pod");
        pod.Metadata!.CreationTimestamp = null;

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.CreatedAt.Should().BeNull();
    }

    [Fact]
    public void MapPodToResource_WithRunningStartedAt_SetsStartedAt()
    {
        var started = DateTime.UtcNow.AddMinutes(-5);
        var pod = CreateMinimalPod("my-pod");
        pod.Status = new V1PodStatus
        {
            Phase = "Running",
            ContainerStatuses =
            [
                new V1ContainerStatus
                {
                    Image = "img:1", ImageID = "id1", Ready = true, RestartCount = 0, Name = "c1",
                    State = new V1ContainerState { Running = new V1ContainerStateRunning { StartedAt = started } }
                }
            ]
        };

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.StartedAt.Should().NotBeNull();
        resource.StartedAt.ToDateTime().Should().BeCloseTo(started, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void MapPodToResource_NullStartedAt_OmitsStartedAt()
    {
        var pod = CreateMinimalPod("my-pod");

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.StartedAt.Should().BeNull();
    }

    // ──────────────────────────────────────────────
    //  MapPodToResource — Properties
    // ──────────────────────────────────────────────

    [Fact]
    public void MapPodToResource_WithImage_AddsContainerImageProperty()
    {
        var pod = CreateMinimalPod("my-pod");
        pod.Spec = new V1PodSpec { Containers = [new V1Container { Name = "c1", Image = "nginx:latest" }] };

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.Properties.Should().Contain(p => p.Name == "container.image" && p.Value.StringValue == "nginx:latest");
    }

    [Fact]
    public void MapPodToResource_NullImage_OmitsContainerImageProperty()
    {
        var pod = CreateMinimalPod("my-pod");

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.Properties.Should().NotContain(p => p.Name == "container.image");
    }

    [Fact]
    public void MapPodToResource_WithPodIp_AddsPodIpProperty()
    {
        var pod = CreateMinimalPod("my-pod");
        pod.Status!.PodIP = "10.0.0.1";

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.Properties.Should().Contain(p => p.Name == "pod.ip" && p.Value.StringValue == "10.0.0.1");
    }

    [Fact]
    public void MapPodToResource_NullPodIp_OmitsPodIpProperty()
    {
        var pod = CreateMinimalPod("my-pod");

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.Properties.Should().NotContain(p => p.Name == "pod.ip");
    }

    [Fact]
    public void MapPodToResource_WithNodeName_AddsPodNodeProperty()
    {
        var pod = CreateMinimalPod("my-pod");
        pod.Spec = new V1PodSpec { Containers = [new V1Container { Name = "c1" }], NodeName = "node-1" };

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.Properties.Should().Contain(p => p.Name == "pod.node" && p.Value.StringValue == "node-1");
    }

    [Fact]
    public void MapPodToResource_WithContainerStatus_AddsRestartCountProperty()
    {
        var pod = CreateMinimalPod("my-pod");
        pod.Status = new V1PodStatus
        {
            Phase = "Running",
            ContainerStatuses = [CreateContainerStatus(ready: true, running: true, restartCount: 3)]
        };

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.Properties.Should().Contain(p => p.Name == "container.restarts" && p.Value.NumberValue == 3);
    }

    // ──────────────────────────────────────────────
    //  MapPodToResource — Health reports
    // ──────────────────────────────────────────────

    [Fact]
    public void MapPodToResource_WithConditionTrue_AddsHealthyReport()
    {
        var pod = CreateMinimalPod("my-pod");
        pod.Status!.Conditions =
        [
            new V1PodCondition { Status = "True", Type = "Ready", Message = "pod is ready" }
        ];

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.HealthReports.Should().ContainSingle()
            .Which.Should().Match<Aspire.DashboardService.Proto.V1.HealthReport>(r =>
                r.Key == "Ready" &&
                r.Status == Aspire.DashboardService.Proto.V1.HealthStatus.Healthy &&
                r.Description == "pod is ready");
    }

    [Fact]
    public void MapPodToResource_WithConditionFalse_AddsUnhealthyReport()
    {
        var pod = CreateMinimalPod("my-pod");
        pod.Status!.Conditions =
        [
            new V1PodCondition { Status = "False", Type = "Ready", Message = "containers not ready" }
        ];

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.HealthReports.Should().ContainSingle()
            .Which.Status.Should().Be(Aspire.DashboardService.Proto.V1.HealthStatus.Unhealthy);
    }

    [Fact]
    public void MapPodToResource_NullConditions_NoHealthReports()
    {
        var pod = CreateMinimalPod("my-pod");

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.HealthReports.Should().BeEmpty();
    }

    // ──────────────────────────────────────────────
    //  MapPodToResource — Commands
    // ──────────────────────────────────────────────

    [Fact]
    public void MapPodToResource_HasRestartCommand()
    {
        var pod = CreateMinimalPod("my-pod");

        var resource = ResourceStateMapper.MapPodToResource(pod);

        resource.Commands.Should().ContainSingle()
            .Which.Should().Match<Aspire.DashboardService.Proto.V1.ResourceCommand>(c =>
                c.Name == "restart" &&
                c.ConfirmationMessage.Contains("my-pod"));
    }

    // ──────────────────────────────────────────────
    //  MapPodToResource — Edge case: minimal pod
    // ──────────────────────────────────────────────

    [Fact]
    public void MapPodToResource_MinimalPodWithAllNulls_DoesNotThrow()
    {
        var pod = new V1Pod();

        var act = () => ResourceStateMapper.MapPodToResource(pod);

        act.Should().NotThrow();
    }

    // ──────────────────────────────────────────────
    //  Helpers
    // ──────────────────────────────────────────────

    private static V1Pod CreateMinimalPod(string name) => new()
    {
        Metadata = new V1ObjectMeta { Name = name, Uid = Guid.NewGuid().ToString() },
        Status = new V1PodStatus { Phase = "Running" }
    };

    private static V1ContainerStatus CreateContainerStatus(
        bool ready = false,
        bool running = false,
        int restartCount = 0,
        string? waitingReason = null,
        string? terminatedReason = null)
    {
        var state = new V1ContainerState();
        if (running)
            state.Running = new V1ContainerStateRunning();
        else if (waitingReason is not null || (!running && terminatedReason is null))
            state.Waiting = new V1ContainerStateWaiting { Reason = waitingReason };
        if (terminatedReason is not null)
            state.Terminated = new V1ContainerStateTerminated { Reason = terminatedReason };

        return new V1ContainerStatus
        {
            Image = "img:1",
            ImageID = "id1",
            Ready = ready,
            RestartCount = restartCount,
            Name = "c1",
            State = state
        };
    }

    private static V1Pod CreatePodWithWaitingContainer(string? reason) => new()
    {
        Status = new V1PodStatus
        {
            Phase = "Pending",
            ContainerStatuses =
            [
                new V1ContainerStatus
                {
                    Image = "img:1", ImageID = "id1", Ready = false, RestartCount = 0, Name = "c1",
                    State = new V1ContainerState { Waiting = new V1ContainerStateWaiting { Reason = reason } }
                }
            ]
        }
    };

    private static V1Pod CreatePodWithTerminatedContainer(string? reason) => new()
    {
        Status = new V1PodStatus
        {
            Phase = "Succeeded",
            ContainerStatuses =
            [
                new V1ContainerStatus
                {
                    Image = "img:1", ImageID = "id1", Ready = false, RestartCount = 0, Name = "c1",
                    State = new V1ContainerState { Terminated = new V1ContainerStateTerminated { Reason = reason } }
                }
            ]
        }
    };
}

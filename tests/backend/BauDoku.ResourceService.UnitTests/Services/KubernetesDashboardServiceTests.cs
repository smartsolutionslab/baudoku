using Aspire.DashboardService.Proto.V1;
using AwesomeAssertions;
using BauDoku.ResourceService.Services;
using Grpc.Core;
using Grpc.Core.Testing;
using k8s;
using k8s.Autorest;
using k8s.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.ResourceService.UnitTests.Services;

public sealed class KubernetesDashboardServiceTests
{
    private readonly ICoreV1Operations coreV1 = Substitute.For<ICoreV1Operations>();
    private readonly IKubernetes kubernetes = Substitute.For<IKubernetes>();

    public KubernetesDashboardServiceTests()
    {
        kubernetes.CoreV1.Returns(coreV1);
    }

    private KubernetesDashboardService CreateService(KubernetesOptions? kubernetesOptions = null)
    {
        var options = Options.Create(kubernetesOptions ?? new KubernetesOptions());

        var podWatcher = new PodWatcher(kubernetes, options, NullLogger<PodWatcher>.Instance);
        var podLogStreamer = new PodLogStreamer(kubernetes, options, NullLogger<PodLogStreamer>.Instance);

        return new KubernetesDashboardService(
            podWatcher,
            podLogStreamer,
            kubernetes,
            options,
            NullLogger<KubernetesDashboardService>.Instance);
    }

    private static ServerCallContext CreateTestCallContext()
    {
        return TestServerCallContext.Create(
            method: "test",
            host: "localhost",
            deadline: DateTime.UtcNow.AddMinutes(1),
            requestHeaders: [],
            cancellationToken: CancellationToken.None,
            peer: "127.0.0.1",
            authContext: null,
            contextPropagationToken: null,
            writeHeadersFunc: _ => Task.CompletedTask,
            writeOptionsGetter: () => new WriteOptions(),
            writeOptionsSetter: _ => { });
    }

    [Fact]
    public async Task GetApplicationInformation_ReturnsConfiguredName()
    {
        var service = CreateService(new KubernetesOptions { Namespace = "staging", ApplicationName = "BauDoku Staging" });

        var response = await service.GetApplicationInformation(
            new ApplicationInformationRequest(),
            CreateTestCallContext());

        response.ApplicationName.Should().Be("BauDoku Staging");
    }

    [Fact]
    public async Task GetApplicationInformation_DefaultOptions_UsesDefault()
    {
        var service = CreateService();

        var response = await service.GetApplicationInformation(
            new ApplicationInformationRequest(),
            CreateTestCallContext());

        response.ApplicationName.Should().Be("BauDoku");
    }

    [Fact]
    public async Task ExecuteResourceCommand_Restart_CallsDeletePod_ReturnsSucceeded()
    {
        var service = CreateService();
        var deleteResponse = new HttpOperationResponse<V1Pod>
        {
            Body = new V1Pod(),
            Response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        };
        coreV1.DeleteNamespacedPodWithHttpMessagesAsync(
            Arg.Any<string>(), Arg.Any<string>())
            .ReturnsForAnyArgs(Task.FromResult(deleteResponse));

        var response = await service.ExecuteResourceCommand(
            new ResourceCommandRequest { CommandName = "restart", ResourceName = "my-pod" },
            CreateTestCallContext());

        response.Kind.Should().Be(ResourceCommandResponseKind.Succeeded);
        await coreV1.ReceivedWithAnyArgs(1).DeleteNamespacedPodWithHttpMessagesAsync(
            Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task ExecuteResourceCommand_Restart_WhenK8sThrows_ReturnsFailed()
    {
        var service = CreateService();
        coreV1.DeleteNamespacedPodWithHttpMessagesAsync(
            Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsyncForAnyArgs(new HttpOperationException("pod not found")
            {
                Response = new HttpResponseMessageWrapper(
                    new HttpResponseMessage(System.Net.HttpStatusCode.NotFound), "")
            });

        var response = await service.ExecuteResourceCommand(
            new ResourceCommandRequest { CommandName = "restart", ResourceName = "missing-pod" },
            CreateTestCallContext());

        response.Kind.Should().Be(ResourceCommandResponseKind.Failed);
        response.ErrorMessage.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExecuteResourceCommand_UnknownCommand_ReturnsFailed()
    {
        var service = CreateService();

        var response = await service.ExecuteResourceCommand(
            new ResourceCommandRequest { CommandName = "scale", ResourceName = "my-pod" },
            CreateTestCallContext());

        response.Kind.Should().Be(ResourceCommandResponseKind.Failed);
        response.ErrorMessage.Should().Contain("Unknown command");
    }
}

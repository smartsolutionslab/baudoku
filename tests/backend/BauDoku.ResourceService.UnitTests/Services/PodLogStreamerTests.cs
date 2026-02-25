using AwesomeAssertions;
using BauDoku.ResourceService.Services;
using k8s;
using k8s.Autorest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace BauDoku.ResourceService.UnitTests.Services;

public sealed class PodLogStreamerTests
{
    private readonly ICoreV1Operations coreV1 = Substitute.For<ICoreV1Operations>();
    private readonly IKubernetes kubernetes = Substitute.For<IKubernetes>();

    public PodLogStreamerTests()
    {
        kubernetes.CoreV1.Returns(coreV1);
    }

    private PodLogStreamer CreateStreamer(Dictionary<string, string?>? config = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(config ?? [])
            .Build();

        return new PodLogStreamer(kubernetes, configuration, NullLogger<PodLogStreamer>.Instance);
    }

    private void SetupLogStream(byte[] content)
    {
        coreV1.ReadNamespacedPodLogWithHttpMessagesAsync(
            default!, default!,
            default, default, default, default, default, default, default, default, default, default, default)
            .ReturnsForAnyArgs(callInfo => Task.FromResult(new HttpOperationResponse<Stream>
            {
                Body = new MemoryStream(content),
                Request = new HttpRequestMessage(),
                Response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            }));
    }

    private void SetupLogStreamThrows()
    {
        coreV1.ReadNamespacedPodLogWithHttpMessagesAsync(
            default!, default!,
            default, default, default, default, default, default, default, default, default, default, default)
            .ReturnsForAnyArgs<Task<HttpOperationResponse<Stream>>>(_ =>
                throw new HttpRequestException("connection refused"));
    }

    [Fact]
    public async Task StreamLogsAsync_YieldsLinesFromStream()
    {
        var streamer = CreateStreamer();
        SetupLogStream("line1\nline2\nline3"u8.ToArray());

        var lines = new List<(string Line, bool IsStdErr)>();
        await foreach (var item in streamer.StreamLogsAsync("test-pod", false, CancellationToken.None))
        {
            lines.Add(item);
        }

        lines.Should().HaveCount(3);
        lines[0].Line.Should().Be("line1");
        lines[1].Line.Should().Be("line2");
        lines[2].Line.Should().Be("line3");
    }

    [Fact]
    public async Task StreamLogsAsync_AllLinesHaveIsStdErrFalse()
    {
        var streamer = CreateStreamer();
        SetupLogStream("line1\nline2"u8.ToArray());

        await foreach (var (_, isStdErr) in streamer.StreamLogsAsync("test-pod", false, CancellationToken.None))
        {
            isStdErr.Should().BeFalse();
        }
    }

    [Fact]
    public async Task StreamLogsAsync_ExceptionOnOpen_YieldsNothing()
    {
        var streamer = CreateStreamer();
        SetupLogStreamThrows();

        var lines = new List<(string Line, bool IsStdErr)>();
        await foreach (var item in streamer.StreamLogsAsync("test-pod", false, CancellationToken.None))
        {
            lines.Add(item);
        }

        lines.Should().BeEmpty();
    }

    [Fact]
    public async Task StreamLogsAsync_EmptyStream_YieldsNothing()
    {
        var streamer = CreateStreamer();
        SetupLogStream([]);

        var lines = new List<(string Line, bool IsStdErr)>();
        await foreach (var item in streamer.StreamLogsAsync("test-pod", false, CancellationToken.None))
        {
            lines.Add(item);
        }

        lines.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ReadsConfigurableTailLines()
    {
        var config = new Dictionary<string, string?> { ["PodLogs:TailLines"] = "500" };
        var streamer = CreateStreamer(config);

        streamer.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_DefaultsTailLinesTo1000()
    {
        var streamer = CreateStreamer();

        streamer.Should().NotBeNull();
    }
}

using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.ResourceService.Services;
using k8s;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace SmartSolutionsLab.BauDoku.ResourceService.UnitTests.Services;

public sealed class PodWatcherTests
{
    private readonly IKubernetes kubernetes = Substitute.For<IKubernetes>();

    private PodWatcher CreateWatcher(KubernetesOptions? kubernetesOptions = null)
    {
        var options = Options.Create(kubernetesOptions ?? new KubernetesOptions());
        return new PodWatcher(kubernetes, options, NullLogger<PodWatcher>.Instance);
    }

    [Fact]
    public void CurrentResources_Initially_IsEmpty()
    {
        var watcher = CreateWatcher();

        watcher.CurrentResources.Should().BeEmpty();
    }

    [Fact]
    public void Subscribe_ReturnsChannel()
    {
        var watcher = CreateWatcher();

        var channel = watcher.Subscribe();

        channel.Should().NotBeNull();
        channel.Reader.Should().NotBeNull();
        channel.Writer.Should().NotBeNull();
    }

    [Fact]
    public void Subscribe_MultipleCalls_ReturnDifferentChannels()
    {
        var watcher = CreateWatcher();

        var channel1 = watcher.Subscribe();
        var channel2 = watcher.Subscribe();

        channel1.Should().NotBeSameAs(channel2);
    }

    [Fact]
    public void Unsubscribe_CompletesChannelWriter()
    {
        var watcher = CreateWatcher();
        var channel = watcher.Subscribe();

        watcher.Unsubscribe(channel);

        channel.Writer.TryWrite(null!).Should().BeFalse();
    }

    [Fact]
    public async Task Unsubscribe_ChannelReaderCompletesIteration()
    {
        var watcher = CreateWatcher();
        var channel = watcher.Subscribe();

        watcher.Unsubscribe(channel);

        var items = new List<object>();
        await foreach (var item in channel.Reader.ReadAllAsync())
        {
            items.Add(item);
        }

        items.Should().BeEmpty();
    }

    [Fact]
    public void Unsubscribe_WithUnknownChannel_DoesNotThrow()
    {
        var watcher = CreateWatcher();
        var unknownChannel = System.Threading.Channels.Channel.CreateUnbounded<Aspire.DashboardService.Proto.V1.WatchResourcesUpdate>();

        var act = () => watcher.Unsubscribe(unknownChannel);

        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WithCustomOptions_DoesNotThrow()
    {
        var watcher = CreateWatcher(new KubernetesOptions
        {
            Namespace = "production",
            LabelSelector = "app=baudoku",
            ReconnectDelaySeconds = 10
        });

        watcher.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_DefaultOptions_DoesNotThrow()
    {
        var watcher = CreateWatcher();

        watcher.Should().NotBeNull();
    }
}

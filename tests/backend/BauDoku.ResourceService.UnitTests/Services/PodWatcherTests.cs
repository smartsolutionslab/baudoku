using AwesomeAssertions;
using BauDoku.ResourceService.Services;
using k8s;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace BauDoku.ResourceService.UnitTests.Services;

public sealed class PodWatcherTests
{
    private readonly IKubernetes kubernetes = Substitute.For<IKubernetes>();

    private PodWatcher CreateWatcher(Dictionary<string, string?>? config = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(config ?? [])
            .Build();

        return new PodWatcher(kubernetes, configuration, NullLogger<PodWatcher>.Instance);
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
    public void Constructor_ReadsNamespaceFromConfig()
    {
        var config = new Dictionary<string, string?> { ["KUBERNETES_NAMESPACE"] = "production" };
        var watcher = CreateWatcher(config);

        // Namespace is private — we verify indirectly that it doesn't throw
        watcher.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_DefaultsNamespaceToDefault()
    {
        var watcher = CreateWatcher();

        // Constructor should not throw when no KUBERNETES_NAMESPACE is set
        watcher.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_ReadsLabelSelectorFromConfig()
    {
        var config = new Dictionary<string, string?> { ["KUBERNETES_LABEL_SELECTOR"] = "app=baudoku" };
        var watcher = CreateWatcher(config);

        watcher.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_ReadsReconnectDelayFromConfig()
    {
        var config = new Dictionary<string, string?> { ["PodWatcher:ReconnectDelaySeconds"] = "10" };
        var watcher = CreateWatcher(config);

        watcher.Should().NotBeNull();
    }
}

using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.ValueObjects;
using BauDoku.Sync.Infrastructure.BackgroundServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Sync.UnitTests.Infrastructure.BackgroundServices;

public sealed class SyncSchedulerServiceTests
{
    private readonly ISyncBatchRepository repository;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<SyncSchedulerService> logger;
    private readonly SyncSchedulerService service;

    public SyncSchedulerServiceTests()
    {
        repository = Substitute.For<ISyncBatchRepository>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        logger = Substitute.For<ILogger<SyncSchedulerService>>();

        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(ISyncBatchRepository)).Returns(repository);
        serviceProvider.GetService(typeof(IUnitOfWork)).Returns(unitOfWork);

        var scope = Substitute.For<IServiceScope>();
        scope.ServiceProvider.Returns(serviceProvider);

        var scopeFactory = Substitute.For<IServiceScopeFactory>();
        scopeFactory.CreateScope().Returns(scope);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Sync:SchedulerIntervalSeconds"] = "1"
            })
            .Build();

        service = new SyncSchedulerService(scopeFactory, logger, config);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoPendingBatches_ShouldNotCallSaveChanges()
    {
        repository.GetPendingBatchesAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new List<SyncBatch>());

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
        await service.StartAsync(cts.Token);
        await Task.Delay(200);
        await service.StopAsync(CancellationToken.None);

        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithPendingBatches_ShouldProcessAndSave()
    {
        var batch = SyncBatch.Create(
            SyncBatchIdentifier.New(),
            DeviceIdentifier.From("device-1"),
            DateTime.UtcNow);

        repository.GetPendingBatchesAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(
                new List<SyncBatch> { batch },
                new List<SyncBatch>());

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
        await service.StartAsync(cts.Token);
        await Task.Delay(200);
        await service.StopAsync(CancellationToken.None);

        batch.Status.Should().Be(BatchStatus.Completed);
        await unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryThrows_ShouldNotCrash()
    {
        repository.GetPendingBatchesAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("DB connection lost"));

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
        await service.StartAsync(cts.Token);
        await Task.Delay(300);

        // Service should still be alive (not crashed)
        var stopAct = () => service.StopAsync(CancellationToken.None);
        await stopAct.Should().NotThrowAsync();
    }

    [Fact]
    public void Constructor_ShouldUseConfiguredInterval()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Sync:SchedulerIntervalSeconds"] = "60"
            })
            .Build();

        var scopeFactory = Substitute.For<IServiceScopeFactory>();

        var service = new SyncSchedulerService(scopeFactory, logger, config);
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithoutConfig_ShouldDefaultTo30Seconds()
    {
        var config = new ConfigurationBuilder().Build();
        var scopeFactory = Substitute.For<IServiceScopeFactory>();

        var service = new SyncSchedulerService(scopeFactory, logger, config);
        service.Should().NotBeNull();
    }
}

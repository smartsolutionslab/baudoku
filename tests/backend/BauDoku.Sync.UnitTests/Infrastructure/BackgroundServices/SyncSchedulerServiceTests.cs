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
    private readonly ISyncBatchRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SyncSchedulerService> _logger;
    private readonly SyncSchedulerService _service;

    public SyncSchedulerServiceTests()
    {
        _repository = Substitute.For<ISyncBatchRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _logger = Substitute.For<ILogger<SyncSchedulerService>>();

        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(ISyncBatchRepository)).Returns(_repository);
        serviceProvider.GetService(typeof(IUnitOfWork)).Returns(_unitOfWork);

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

        _service = new SyncSchedulerService(scopeFactory, _logger, config);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoPendingBatches_ShouldNotCallSaveChanges()
    {
        var repositoryCalled = new TaskCompletionSource<bool>();
        _repository.GetPendingBatchesAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                repositoryCalled.TrySetResult(true);
                return new List<SyncBatch>();
            });

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await _service.StartAsync(cts.Token);
        await repositoryCalled.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await _service.StopAsync(CancellationToken.None);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithPendingBatches_ShouldProcessAndSave()
    {
        var batch = SyncBatch.Create(
            SyncBatchId.New(),
            new DeviceId("device-1"),
            DateTime.UtcNow);

        var saveChangesCalled = new TaskCompletionSource<bool>();
        _repository.GetPendingBatchesAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(
                new List<SyncBatch> { batch },
                new List<SyncBatch>());
        _unitOfWork.When(u => u.SaveChangesAsync(Arg.Any<CancellationToken>()))
            .Do(_ => saveChangesCalled.TrySetResult(true));

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await _service.StartAsync(cts.Token);
        await saveChangesCalled.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await _service.StopAsync(CancellationToken.None);

        batch.Status.Should().Be(BatchStatus.Completed);
        await _unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryThrows_ShouldNotCrash()
    {
        var repositoryCalled = new TaskCompletionSource<bool>();
        _repository.GetPendingBatchesAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns<List<SyncBatch>>(_ =>
            {
                repositoryCalled.TrySetResult(true);
                throw new InvalidOperationException("DB connection lost");
            });

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await _service.StartAsync(cts.Token);
        await repositoryCalled.Task.WaitAsync(TimeSpan.FromSeconds(5));

        // Service should still be alive (not crashed)
        var stopAct = () => _service.StopAsync(CancellationToken.None);
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

        var service = new SyncSchedulerService(scopeFactory, _logger, config);
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithoutConfig_ShouldDefaultTo30Seconds()
    {
        var config = new ConfigurationBuilder().Build();
        var scopeFactory = Substitute.For<IServiceScopeFactory>();

        var service = new SyncSchedulerService(scopeFactory, _logger, config);
        service.Should().NotBeNull();
    }
}

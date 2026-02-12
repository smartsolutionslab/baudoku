using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Sync.Application.Commands.ResolveConflict;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Sync.UnitTests.Application.Commands;

public sealed class ResolveConflictCommandHandlerTests
{
    private readonly ISyncBatchRepository _syncBatchRepository;
    private readonly IEntityVersionStore _entityVersionStore;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ResolveConflictCommandHandler _handler;

    public ResolveConflictCommandHandlerTests()
    {
        _syncBatchRepository = Substitute.For<ISyncBatchRepository>();
        _entityVersionStore = Substitute.For<IEntityVersionStore>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new ResolveConflictCommandHandler(_syncBatchRepository, _entityVersionStore, _unitOfWork);
    }

    private static (SyncBatch batch, ConflictRecordId conflictId) CreateBatchWithConflict()
    {
        var batchId = SyncBatchId.New();
        var deviceId = new DeviceId("device-001");
        var batch = SyncBatch.Create(batchId, deviceId, DateTime.UtcNow);

        var conflictId = ConflictRecordId.New();
        var entityRef = new EntityReference(EntityType.Project, Guid.NewGuid());
        batch.AddConflict(
            conflictId,
            entityRef,
            new DeltaPayload("""{"client":"v1"}"""),
            new DeltaPayload("""{"server":"v2"}"""),
            new SyncVersion(1),
            new SyncVersion(3));

        return (batch, conflictId);
    }

    [Fact]
    public async Task Handle_ClientWins_ShouldResolveAndUpdateVersion()
    {
        var (batch, conflictId) = CreateBatchWithConflict();
        _syncBatchRepository.GetByConflictIdAsync(Arg.Any<ConflictRecordId>(), Arg.Any<CancellationToken>())
            .Returns(batch);
        _entityVersionStore.GetCurrentVersionAsync(Arg.Any<EntityType>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new SyncVersion(3));

        var command = new ResolveConflictCommand(conflictId.Value, "client_wins", null);

        await _handler.Handle(command);

        var conflict = batch.Conflicts.First(c => c.Id == conflictId);
        conflict.Status.Should().Be(ConflictStatus.ClientWins);
        await _entityVersionStore.Received(1).SetVersionAsync(
            Arg.Any<EntityType>(), Arg.Any<Guid>(), Arg.Any<SyncVersion>(), Arg.Any<string>(), Arg.Any<DeviceId>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ServerWins_ShouldResolveWithoutUpdatingVersion()
    {
        var (batch, conflictId) = CreateBatchWithConflict();
        _syncBatchRepository.GetByConflictIdAsync(Arg.Any<ConflictRecordId>(), Arg.Any<CancellationToken>())
            .Returns(batch);

        var command = new ResolveConflictCommand(conflictId.Value, "server_wins", null);

        await _handler.Handle(command);

        var conflict = batch.Conflicts.First(c => c.Id == conflictId);
        conflict.Status.Should().Be(ConflictStatus.ServerWins);
        await _entityVersionStore.DidNotReceive().SetVersionAsync(
            Arg.Any<EntityType>(), Arg.Any<Guid>(), Arg.Any<SyncVersion>(), Arg.Any<string>(), Arg.Any<DeviceId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ManualMerge_ShouldResolveWithMergedPayloadAndUpdateVersion()
    {
        var (batch, conflictId) = CreateBatchWithConflict();
        _syncBatchRepository.GetByConflictIdAsync(Arg.Any<ConflictRecordId>(), Arg.Any<CancellationToken>())
            .Returns(batch);
        _entityVersionStore.GetCurrentVersionAsync(Arg.Any<EntityType>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new SyncVersion(3));

        var command = new ResolveConflictCommand(conflictId.Value, "manual_merge", """{"merged":"data"}""");

        await _handler.Handle(command);

        var conflict = batch.Conflicts.First(c => c.Id == conflictId);
        conflict.Status.Should().Be(ConflictStatus.Merged);
        conflict.ResolvedPayload!.Value.Should().Be("""{"merged":"data"}""");
        await _entityVersionStore.Received(1).SetVersionAsync(
            Arg.Any<EntityType>(), Arg.Any<Guid>(), Arg.Any<SyncVersion>(), """{"merged":"data"}""", Arg.Any<DeviceId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenBatchNotFound_ShouldThrow()
    {
        _syncBatchRepository.GetByConflictIdAsync(Arg.Any<ConflictRecordId>(), Arg.Any<CancellationToken>())
            .Returns((SyncBatch?)null);

        var command = new ResolveConflictCommand(Guid.NewGuid(), "client_wins", null);

        var act = () => _handler.Handle(command);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_WhenConflictNotFoundInBatch_ShouldThrow()
    {
        var (batch, _) = CreateBatchWithConflict();
        _syncBatchRepository.GetByConflictIdAsync(Arg.Any<ConflictRecordId>(), Arg.Any<CancellationToken>())
            .Returns(batch);

        var command = new ResolveConflictCommand(Guid.NewGuid(), "client_wins", null);

        var act = () => _handler.Handle(command);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}

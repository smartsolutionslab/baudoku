using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Sync.Application.Commands.ResolveConflict;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Sync.UnitTests.Application.Commands;

public sealed class ResolveConflictCommandHandlerTests
{
    private readonly ISyncBatchRepository syncBatches;
    private readonly IEntityVersionStore entityVersionStore;
    private readonly IUnitOfWork unitOfWork;
    private readonly ResolveConflictCommandHandler handler;

    public ResolveConflictCommandHandlerTests()
    {
        syncBatches = Substitute.For<ISyncBatchRepository>();
        entityVersionStore = Substitute.For<IEntityVersionStore>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        handler = new ResolveConflictCommandHandler(syncBatches, entityVersionStore, unitOfWork);
    }

    private static (SyncBatch batch, ConflictRecordIdentifier conflictId) CreateBatchWithConflict()
    {
        var batchId = SyncBatchIdentifier.New();
        var deviceId = DeviceIdentifier.From("device-001");
        var batch = SyncBatch.Create(batchId, deviceId, DateTime.UtcNow);

        var conflictId = ConflictRecordIdentifier.New();
        var entityRef = EntityReference.Create(EntityType.Project, Guid.NewGuid());
        batch.AddConflict(
            conflictId,
            entityRef,
            DeltaPayload.From("""{"client":"v1"}"""),
            DeltaPayload.From("""{"server":"v2"}"""),
            SyncVersion.From(1),
            SyncVersion.From(3));

        return (batch, conflictId);
    }

    [Fact]
    public async Task Handle_ClientWins_ShouldResolveAndUpdateVersion()
    {
        var (batch, conflictId) = CreateBatchWithConflict();
        syncBatches.GetByConflictIdAsync(Arg.Any<ConflictRecordIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(batch);
        entityVersionStore.GetCurrentVersionAsync(Arg.Any<EntityReference>(), Arg.Any<CancellationToken>())
            .Returns(SyncVersion.From(3));

        var command = new ResolveConflictCommand(conflictId.Value, "client_wins", null);

        await handler.Handle(command);

        var conflict = batch.Conflicts.First(c => c.Id == conflictId);
        conflict.Status.Should().Be(ConflictStatus.ClientWins);
        await entityVersionStore.Received(1).SetVersionAsync(
            Arg.Any<EntityReference>(), Arg.Any<SyncVersion>(), Arg.Any<string>(), Arg.Any<DeviceIdentifier>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ServerWins_ShouldResolveWithoutUpdatingVersion()
    {
        var (batch, conflictId) = CreateBatchWithConflict();
        syncBatches.GetByConflictIdAsync(Arg.Any<ConflictRecordIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(batch);

        var command = new ResolveConflictCommand(conflictId.Value, "server_wins", null);

        await handler.Handle(command);

        var conflict = batch.Conflicts.First(c => c.Id == conflictId);
        conflict.Status.Should().Be(ConflictStatus.ServerWins);
        await entityVersionStore.DidNotReceive().SetVersionAsync(
            Arg.Any<EntityReference>(), Arg.Any<SyncVersion>(), Arg.Any<string>(), Arg.Any<DeviceIdentifier>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ManualMerge_ShouldResolveWithMergedPayloadAndUpdateVersion()
    {
        var (batch, conflictId) = CreateBatchWithConflict();
        syncBatches.GetByConflictIdAsync(Arg.Any<ConflictRecordIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(batch);
        entityVersionStore.GetCurrentVersionAsync(Arg.Any<EntityReference>(), Arg.Any<CancellationToken>())
            .Returns(SyncVersion.From(3));

        var command = new ResolveConflictCommand(conflictId.Value, "manual_merge", """{"merged":"data"}""");

        await handler.Handle(command);

        var conflict = batch.Conflicts.First(c => c.Id == conflictId);
        conflict.Status.Should().Be(ConflictStatus.Merged);
        conflict.ResolvedPayload!.Value.Should().Be("""{"merged":"data"}""");
        await entityVersionStore.Received(1).SetVersionAsync(
            Arg.Any<EntityReference>(), Arg.Any<SyncVersion>(), """{"merged":"data"}""", Arg.Any<DeviceIdentifier>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenBatchNotFound_ShouldThrow()
    {
        syncBatches.GetByConflictIdAsync(Arg.Any<ConflictRecordIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException("Batch nicht gefunden."));

        var command = new ResolveConflictCommand(Guid.NewGuid(), "client_wins", null);

        var act = () => handler.Handle(command);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenConflictNotFoundInBatch_ShouldThrow()
    {
        var (batch, _) = CreateBatchWithConflict();
        syncBatches.GetByConflictIdAsync(Arg.Any<ConflictRecordIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(batch);

        var command = new ResolveConflictCommand(Guid.NewGuid(), "client_wins", null);

        var act = () => handler.Handle(command);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}

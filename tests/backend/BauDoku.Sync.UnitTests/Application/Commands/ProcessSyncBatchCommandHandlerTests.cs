using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Sync.Application.Commands.ProcessSyncBatch;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Sync.UnitTests.Application.Commands;

public sealed class ProcessSyncBatchCommandHandlerTests
{
    private readonly ISyncBatchRepository syncBatchRepository;
    private readonly IEntityVersionStore entityVersionStore;
    private readonly IUnitOfWork unitOfWork;
    private readonly ProcessSyncBatchCommandHandler handler;

    public ProcessSyncBatchCommandHandlerTests()
    {
        syncBatchRepository = Substitute.For<ISyncBatchRepository>();
        entityVersionStore = Substitute.For<IEntityVersionStore>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        handler = new ProcessSyncBatchCommandHandler(syncBatchRepository, entityVersionStore, unitOfWork);
    }

    private static SyncDeltaDto CreateDelta(Guid? entityId = null, long baseVersion = 0) =>
        new("project", entityId ?? Guid.NewGuid(), "create", baseVersion, """{"name":"Test"}""", DateTime.UtcNow);

    [Fact]
    public async Task Handle_AllDeltasApplied_ShouldReturnCompleted()
    {
        var entityId = Guid.NewGuid();
        entityVersionStore.GetCurrentVersionAsync(Arg.Any<EntityType>(), entityId, Arg.Any<CancellationToken>())
            .Returns(SyncVersion.Initial);

        var command = new ProcessSyncBatchCommand("device-001", [CreateDelta(entityId, 0)]);

        var result = await handler.Handle(command);

        result.AppliedCount.Should().Be(1);
        result.ConflictCount.Should().Be(0);
        result.BatchId.Should().NotBe(Guid.Empty);
        await entityVersionStore.Received(1).SetVersionAsync(
            Arg.Any<EntityType>(), entityId, Arg.Any<SyncVersion>(), Arg.Any<string>(), Arg.Any<DeviceIdentifier>(), Arg.Any<CancellationToken>());
        await syncBatchRepository.Received(1).AddAsync(Arg.Any<SyncBatch>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_VersionMismatch_ShouldCreateConflict()
    {
        var entityId = Guid.NewGuid();
        entityVersionStore.GetCurrentVersionAsync(Arg.Any<EntityType>(), entityId, Arg.Any<CancellationToken>())
            .Returns(SyncVersion.From(5));
        entityVersionStore.GetCurrentPayloadAsync(Arg.Any<EntityType>(), entityId, Arg.Any<CancellationToken>())
            .Returns("""{"name":"Server"}""");

        var command = new ProcessSyncBatchCommand("device-001", [CreateDelta(entityId, 0)]);

        var result = await handler.Handle(command);

        result.AppliedCount.Should().Be(0);
        result.ConflictCount.Should().Be(1);
        result.Conflicts.Should().ContainSingle();
        result.Conflicts[0].ServerPayload.Should().Be("""{"name":"Server"}""");
        result.Conflicts[0].ClientVersion.Should().Be(0);
        result.Conflicts[0].ServerVersion.Should().Be(5);
    }

    [Fact]
    public async Task Handle_AllConflicts_ShouldReturnFailed()
    {
        var entityId1 = Guid.NewGuid();
        var entityId2 = Guid.NewGuid();

        entityVersionStore.GetCurrentVersionAsync(Arg.Any<EntityType>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(SyncVersion.From(3));
        entityVersionStore.GetCurrentPayloadAsync(Arg.Any<EntityType>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns("{}");

        var command = new ProcessSyncBatchCommand("device-001",
            [CreateDelta(entityId1, 0), CreateDelta(entityId2, 0)]);

        var result = await handler.Handle(command);

        result.AppliedCount.Should().Be(0);
        result.ConflictCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_MixedResult_ShouldReturnPartialConflict()
    {
        var appliedEntityId = Guid.NewGuid();
        var conflictEntityId = Guid.NewGuid();

        entityVersionStore.GetCurrentVersionAsync(Arg.Any<EntityType>(), appliedEntityId, Arg.Any<CancellationToken>())
            .Returns(SyncVersion.Initial);
        entityVersionStore.GetCurrentVersionAsync(Arg.Any<EntityType>(), conflictEntityId, Arg.Any<CancellationToken>())
            .Returns(SyncVersion.From(5));
        entityVersionStore.GetCurrentPayloadAsync(Arg.Any<EntityType>(), conflictEntityId, Arg.Any<CancellationToken>())
            .Returns("{}");

        var command = new ProcessSyncBatchCommand("device-001",
            [CreateDelta(appliedEntityId, 0), CreateDelta(conflictEntityId, 0)]);

        var result = await handler.Handle(command);

        result.AppliedCount.Should().Be(1);
        result.ConflictCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_MultipleDeltasApplied_ShouldIncrementVersions()
    {
        var entityId1 = Guid.NewGuid();
        var entityId2 = Guid.NewGuid();

        entityVersionStore.GetCurrentVersionAsync(Arg.Any<EntityType>(), entityId1, Arg.Any<CancellationToken>())
            .Returns(SyncVersion.Initial);
        entityVersionStore.GetCurrentVersionAsync(Arg.Any<EntityType>(), entityId2, Arg.Any<CancellationToken>())
            .Returns(SyncVersion.From(2));

        var command = new ProcessSyncBatchCommand("device-001",
            [CreateDelta(entityId1, 0), CreateDelta(entityId2, 2)]);

        var result = await handler.Handle(command);

        result.AppliedCount.Should().Be(2);
        result.ConflictCount.Should().Be(0);
        await entityVersionStore.Received(2).SetVersionAsync(
            Arg.Any<EntityType>(), Arg.Any<Guid>(), Arg.Any<SyncVersion>(), Arg.Any<string>(), Arg.Any<DeviceIdentifier>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NullServerPayload_ShouldUseEmptyJson()
    {
        var entityId = Guid.NewGuid();
        entityVersionStore.GetCurrentVersionAsync(Arg.Any<EntityType>(), entityId, Arg.Any<CancellationToken>())
            .Returns(SyncVersion.From(1));
        entityVersionStore.GetCurrentPayloadAsync(Arg.Any<EntityType>(), entityId, Arg.Any<CancellationToken>())
            .Returns((string?)null);

        var command = new ProcessSyncBatchCommand("device-001", [CreateDelta(entityId, 0)]);

        var result = await handler.Handle(command);

        result.Conflicts.Should().ContainSingle();
        result.Conflicts[0].ServerPayload.Should().Be("{}");
    }
}

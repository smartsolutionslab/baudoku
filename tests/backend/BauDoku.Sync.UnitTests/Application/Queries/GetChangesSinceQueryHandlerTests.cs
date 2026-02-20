using AwesomeAssertions;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Application.Queries;
using BauDoku.Sync.Application.Queries.Handlers;
using BauDoku.Sync.Domain;
using NSubstitute;

namespace BauDoku.Sync.UnitTests.Application.Queries;

public sealed class GetChangesSinceQueryHandlerTests
{
    private readonly IEntityVersionReadStore readStore;
    private readonly GetChangesSinceQueryHandler handler;

    public GetChangesSinceQueryHandlerTests()
    {
        readStore = Substitute.For<IEntityVersionReadStore>();
        handler = new GetChangesSinceQueryHandler(readStore);
    }

    private static ServerDeltaDto CreateDelta(int i = 1) =>
        new("project", Guid.NewGuid(), "update", i, """{"name":"Test"}""", DateTime.UtcNow);

    [Fact]
    public async Task Handle_ShouldReturnChanges()
    {
        var changes = new List<ServerDeltaDto> { CreateDelta() };
        readStore.GetChangedSinceAsync(Arg.Any<DateTime?>(), Arg.Any<DeviceIdentifier>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(changes);

        var query = new GetChangesSinceQuery(DeviceIdentifier.From("device-001"), DateTime.UtcNow.AddHours(-1), 100);

        var result = await handler.Handle(query);

        result.Changes.Should().ContainSingle();
        result.HasMore.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenMoreChangesAvailable_ShouldSetHasMore()
    {
        // Request limit=2, handler requests 3 (limit+1), returns 3 → hasMore=true, trims to 2
        var changes = new List<ServerDeltaDto> { CreateDelta(1), CreateDelta(2), CreateDelta(3) };
        readStore.GetChangedSinceAsync(Arg.Any<DateTime?>(), Arg.Any<DeviceIdentifier>(), 3, Arg.Any<CancellationToken>())
            .Returns(changes);

        var query = new GetChangesSinceQuery(DeviceIdentifier.From("device-001"), null, 2);

        var result = await handler.Handle(query);

        result.Changes.Should().HaveCount(2);
        result.HasMore.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNullLimit_ShouldDefault100()
    {
        readStore.GetChangedSinceAsync(Arg.Any<DateTime?>(), Arg.Any<DeviceIdentifier>(), 101, Arg.Any<CancellationToken>())
            .Returns(new List<ServerDeltaDto>());

        var query = new GetChangesSinceQuery(DeviceIdentifier.From("device-001"), null, null);

        var result = await handler.Handle(query);

        result.Changes.Should().BeEmpty();
        result.HasMore.Should().BeFalse();
        await readStore.Received(1).GetChangedSinceAsync(
            Arg.Any<DateTime?>(), Arg.Any<DeviceIdentifier>(), 101, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldSetServerTimestamp()
    {
        readStore.GetChangedSinceAsync(Arg.Any<DateTime?>(), Arg.Any<DeviceIdentifier>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new List<ServerDeltaDto>());

        var before = DateTime.UtcNow;
        var result = await handler.Handle(new GetChangesSinceQuery(DeviceIdentifier.From("device-001"), null, null));
        var after = DateTime.UtcNow;

        result.ServerTimestamp.Should().BeOnOrAfter(before);
        result.ServerTimestamp.Should().BeOnOrBefore(after);
    }
}

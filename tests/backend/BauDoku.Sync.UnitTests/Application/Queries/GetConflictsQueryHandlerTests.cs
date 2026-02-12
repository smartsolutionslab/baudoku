using AwesomeAssertions;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Application.Queries.GetConflicts;
using BauDoku.Sync.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Sync.UnitTests.Application.Queries;

public sealed class GetConflictsQueryHandlerTests
{
    private readonly ISyncBatchReadRepository _readRepository;
    private readonly GetConflictsQueryHandler _handler;

    public GetConflictsQueryHandlerTests()
    {
        _readRepository = Substitute.For<ISyncBatchReadRepository>();
        _handler = new GetConflictsQueryHandler(_readRepository);
    }

    [Fact]
    public async Task Handle_WithDeviceIdAndStatus_ShouldPassFilters()
    {
        var expected = new List<ConflictDto>
        {
            new(Guid.NewGuid(), "project", Guid.NewGuid(), """{"c":"1"}""", """{"s":"2"}""", 1, 3, "unresolved", DateTime.UtcNow)
        };
        _readRepository.GetConflictsAsync(
            Arg.Is<DeviceId>(d => d.Value == "device-001"),
            Arg.Is<ConflictStatus>(s => s.Value == "unresolved"),
            Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await _handler.Handle(new GetConflictsQuery("device-001", "unresolved"));

        result.Should().BeSameAs(expected);
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_WithNullFilters_ShouldPassNulls()
    {
        _readRepository.GetConflictsAsync(null, null, Arg.Any<CancellationToken>())
            .Returns(new List<ConflictDto>());

        var result = await _handler.Handle(new GetConflictsQuery(null, null));

        result.Should().BeEmpty();
        await _readRepository.Received(1).GetConflictsAsync(null, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithOnlyDeviceId_ShouldPassDeviceIdAndNullStatus()
    {
        _readRepository.GetConflictsAsync(
            Arg.Is<DeviceId>(d => d.Value == "device-002"),
            null,
            Arg.Any<CancellationToken>())
            .Returns(new List<ConflictDto>());

        var result = await _handler.Handle(new GetConflictsQuery("device-002", null));

        result.Should().BeEmpty();
    }
}

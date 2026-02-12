using AwesomeAssertions;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.GetMeasurements;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Queries;

public sealed class GetMeasurementsQueryHandlerTests
{
    private readonly IInstallationRepository _repository;
    private readonly GetMeasurementsQueryHandler _handler;

    public GetMeasurementsQueryHandlerTests()
    {
        _repository = Substitute.For<IInstallationRepository>();
        _handler = new GetMeasurementsQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenInstallationHasMeasurements_ShouldReturnMappedDtos()
    {
        var installation = Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.CableTray,
            new GpsPosition(48.0, 11.0, null, 3.5, "gps"));

        installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.InsulationResistance,
            new MeasurementValue(500.0, "MΩ", 1.0, null),
            "Notiz");

        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var result = await _handler.Handle(new GetMeasurementsQuery(installation.Id.Value), CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Type.Should().Be("insulation_resistance");
        result[0].Value.Should().Be(500.0);
        result[0].Unit.Should().Be("MΩ");
        result[0].Notes.Should().Be("Notiz");
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns((Installation?)null);

        var act = () => _handler.Handle(new GetMeasurementsQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}

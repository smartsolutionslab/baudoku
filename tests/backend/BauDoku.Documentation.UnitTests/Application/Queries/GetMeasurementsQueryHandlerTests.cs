using AwesomeAssertions;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.GetMeasurements;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Queries;

public sealed class GetMeasurementsQueryHandlerTests
{
    private readonly IInstallationRepository installations;
    private readonly GetMeasurementsQueryHandler handler;

    public GetMeasurementsQueryHandlerTests()
    {
        installations = Substitute.For<IInstallationRepository>();
        handler = new GetMeasurementsQueryHandler(installations);
    }

    [Fact]
    public async Task Handle_WhenInstallationHasMeasurements_ShouldReturnMappedDtos()
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(48.0, 11.0, null, 3.5, "gps"));

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.InsulationResistance,
            MeasurementValue.Create(500.0, "MΩ", 1.0, null),
            "Notiz");

        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var result = await handler.Handle(new GetMeasurementsQuery(installation.Id.Value), CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Type.Should().Be("insulation_resistance");
        result[0].Value.Should().Be(500.0);
        result[0].Unit.Should().Be("MΩ");
        result[0].Notes.Should().Be("Notiz");
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns((Installation?)null);

        var act = () => handler.Handle(new GetMeasurementsQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

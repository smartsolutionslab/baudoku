using AwesomeAssertions;
using BauDoku.Documentation.Application.Queries;
using BauDoku.Documentation.Application.ReadModel;
using BauDoku.Documentation.Application.Queries.Handlers;
using BauDoku.Documentation.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Documentation.UnitTests.Application.Queries;

public sealed class GetMeasurementsQueryHandlerTests
{
    private readonly IInstallationReadRepository installations;
    private readonly GetMeasurementsQueryHandler handler;

    public GetMeasurementsQueryHandlerTests()
    {
        installations = Substitute.For<IInstallationReadRepository>();
        handler = new GetMeasurementsQueryHandler(installations);
    }

    [Fact]
    public async Task Handle_WhenInstallationHasMeasurements_ShouldReturnMappedDtos()
    {
        var installationId = InstallationIdentifier.New();
        IReadOnlyList<MeasurementDto> measurements =
        [
            new MeasurementDto(Guid.NewGuid(), installationId.Value, "insulation_resistance", 500.0, "MΩ", 1.0, null, "pass", "Notiz", DateTime.UtcNow)
        ];

        installations.GetMeasurementsAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(measurements);

        var result = await handler.Handle(new GetMeasurementsQuery(installationId));

        result.Should().ContainSingle();
        result[0].Type.Should().Be("insulation_resistance");
        result[0].Value.Should().Be(500.0);
        result[0].Unit.Should().Be("MΩ");
        result[0].Notes.Should().Be("Notiz");
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        installations.GetMeasurementsAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException());

        var act = () => handler.Handle(new GetMeasurementsQuery(InstallationIdentifier.New()));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

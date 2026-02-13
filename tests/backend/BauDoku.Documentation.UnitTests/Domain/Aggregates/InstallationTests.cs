using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.Events;
using BauDoku.Documentation.Domain.ValueObjects;
using BauDoku.Documentation.UnitTests.Builders;

namespace BauDoku.Documentation.UnitTests.Domain.Aggregates;

public sealed class InstallationTests
{
    private static Installation CreateValidInstallation() => new InstallationBuilder().Build();

    [Fact]
    public void Create_ShouldSetProperties()
    {
        var installation = CreateValidInstallation();

        installation.Id.Should().NotBeNull();
        installation.ProjectId.Should().NotBe(Guid.Empty);
        installation.ZoneId.Should().NotBeNull();
        installation.Type.Should().Be(InstallationType.CableTray);
        installation.Position.Latitude.Should().Be(48.1351);
        installation.Description!.Value.Should().Be("Kabeltrasse im Erdgeschoss");
        installation.CableSpec!.CableType.Should().Be("NYM-J 5x2.5");
        installation.Depth!.ValueInMillimeters.Should().Be(600);
        installation.Manufacturer!.Value.Should().Be("Hager");
        installation.ModelName!.Value.Should().Be("VZ312N");
        installation.SerialNumber!.Value.Should().Be("SN-12345");
    }

    [Fact]
    public void Create_ShouldSetStatusToInProgress()
    {
        var installation = CreateValidInstallation();
        installation.Status.Should().Be(InstallationStatus.InProgress);
    }

    [Fact]
    public void Create_ShouldSetQualityGrade()
    {
        var installation = CreateValidInstallation();
        installation.QualityGrade.Should().NotBeNull();
        installation.QualityGrade.Should().Be(GpsQualityGrade.B);
    }

    [Fact]
    public void Create_ShouldRaiseInstallationDocumentedEvent()
    {
        var installation = CreateValidInstallation();
        installation.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<InstallationDocumented>();
    }

    [Fact]
    public void Create_WithInvalidGpsAccuracy_ShouldThrowBusinessRuleException()
    {
        var act = () => Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.JunctionBox,
            new GpsPosition(48.0, 11.0, null, 150.0, "internal_gps"));

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void Create_WithOptionalFieldsNull_ShouldSucceed()
    {
        var installation = Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.Grounding,
            new GpsPosition(48.0, 11.0, null, 5.0, "internal_gps"));

        installation.Description.Should().BeNull();
        installation.CableSpec.Should().BeNull();
        installation.Depth.Should().BeNull();
        installation.Manufacturer.Should().BeNull();
        installation.ModelName.Should().BeNull();
        installation.SerialNumber.Should().BeNull();
        installation.ZoneId.Should().BeNull();
    }

    [Fact]
    public void MarkAsCompleted_ShouldSetStatusAndTimestamp()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.MarkAsCompleted();

        installation.Status.Should().Be(InstallationStatus.Completed);
        installation.CompletedAt.Should().NotBeNull();
        installation.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<InstallationCompleted>();
    }

    [Fact]
    public void MarkAsCompleted_WhenAlreadyCompleted_ShouldThrowBusinessRuleException()
    {
        var installation = CreateValidInstallation();
        installation.MarkAsCompleted();

        var act = () => installation.MarkAsCompleted();
        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void Create_WithLowQualityGps_ShouldRaiseLowGpsQualityDetectedEvent()
    {
        var installation = Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.JunctionBox,
            new GpsPosition(48.0, 11.0, null, 50.0, "internal_gps"));

        installation.QualityGrade.Should().Be(GpsQualityGrade.D);
        installation.DomainEvents.Should().HaveCount(2);
        installation.DomainEvents.Should().ContainSingle(e => e is LowGpsQualityDetected);
    }

    [Fact]
    public void Create_WithGoodQualityGps_ShouldNotRaiseLowGpsQualityDetectedEvent()
    {
        var installation = CreateValidInstallation();

        installation.QualityGrade.Should().NotBe(GpsQualityGrade.D);
        installation.DomainEvents.Should().NotContain(e => e is LowGpsQualityDetected);
    }

    [Fact]
    public void Create_ShouldHaveEmptyPhotosAndMeasurements()
    {
        var installation = CreateValidInstallation();
        installation.Photos.Should().BeEmpty();
        installation.Measurements.Should().BeEmpty();
    }
}

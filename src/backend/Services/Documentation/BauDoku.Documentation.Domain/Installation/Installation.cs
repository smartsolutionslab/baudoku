using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.Entities;
using BauDoku.Documentation.Domain.Events;
using BauDoku.Documentation.Domain.Rules;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Aggregates;

public sealed class Installation : AggregateRoot<InstallationIdentifier>
{
    private readonly List<Photo> photos = [];
    private readonly List<Measurement> measurements = [];

    public ProjectIdentifier ProjectId { get; private set; } = default!;
    public ZoneIdentifier? ZoneId { get; private set; }
    public InstallationType Type { get; private set; } = default!;
    public InstallationStatus Status { get; private set; } = default!;
    public GpsPosition Position { get; private set; } = default!;
    public Description? Description { get; private set; }
    public CableSpec? CableSpec { get; private set; }
    public Depth? Depth { get; private set; }
    public Manufacturer? Manufacturer { get; private set; }
    public ModelName? ModelName { get; private set; }
    public SerialNumber? SerialNumber { get; private set; }
    public GpsQualityGrade QualityGrade { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public IReadOnlyList<Photo> Photos => photos.AsReadOnly();
    public IReadOnlyList<Measurement> Measurements => measurements.AsReadOnly();

    private Installation() { }

    public static Installation Create(
        InstallationIdentifier id,
        ProjectIdentifier projectId,
        ZoneIdentifier? zoneId,
        InstallationType type,
        GpsPosition position,
        Description? description = null,
        CableSpec? cableSpec = null,
        Depth? depth = null,
        Manufacturer? manufacturer = null,
        ModelName? modelName = null,
        SerialNumber? serialNumber = null)
    {
        CheckRule(new InstallationMustHaveValidGpsPosition(position));

        var qualityGrade = position.CalculateQualityGrade();

        var installation = new Installation
        {
            Id = id,
            ProjectId = projectId,
            ZoneId = zoneId,
            Type = type,
            Status = InstallationStatus.InProgress,
            Position = position,
            QualityGrade = qualityGrade,
            Description = description,
            CableSpec = cableSpec,
            Depth = depth,
            Manufacturer = manufacturer,
            ModelName = modelName,
            SerialNumber = serialNumber,
            CreatedAt = DateTime.UtcNow
        };

        installation.AddDomainEvent(new InstallationDocumented(
            id, projectId, type, DateTime.UtcNow));

        if (qualityGrade == GpsQualityGrade.D)
            installation.AddDomainEvent(new LowGpsQualityDetected(id, qualityGrade, DateTime.UtcNow));

        return installation;
    }

    public void AddPhoto(
        PhotoIdentifier photoId,
        FileName fileName,
        BlobUrl blobUrl,
        ContentType contentType,
        FileSize fileSize,
        PhotoType photoType,
        Caption? caption = null,
        Description? description = null,
        GpsPosition? position = null,
        DateTime? takenAt = null)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        var photo = Photo.Create(
            photoId, fileName, blobUrl, contentType, fileSize,
            photoType, caption, description, position, takenAt);

        photos.Add(photo);

        AddDomainEvent(new PhotoAdded(Id, photoId, DateTime.UtcNow));
    }

    public void RemovePhoto(PhotoIdentifier photoId)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        var photo = photos.FirstOrDefault(p => p.Id == photoId) ?? throw new InvalidOperationException($"Foto mit ID {photoId.Value} nicht gefunden.");

        photos.Remove(photo);

        AddDomainEvent(new PhotoRemoved(Id, photoId, DateTime.UtcNow));
    }

    public void RecordMeasurement(MeasurementIdentifier measurementId, MeasurementType type, MeasurementValue value, Notes? notes = null)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));
        CheckRule(new MeasurementValueMustBeNonNegative(value));
        CheckRule(new MeasurementTypeMustMatchInstallationType(Type, type));

        var measurement = Measurement.Create(measurementId, type, value, notes);
        measurements.Add(measurement);

        AddDomainEvent(new MeasurementRecorded(Id, measurementId, DateTime.UtcNow));
    }

    public void RemoveMeasurement(MeasurementIdentifier measurementId)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        var measurement = measurements.FirstOrDefault(m => m.Id == measurementId) ?? throw new InvalidOperationException($"Messung mit ID {measurementId.Value} nicht gefunden.");

        measurements.Remove(measurement);

        AddDomainEvent(new MeasurementRemoved(Id, measurementId, DateTime.UtcNow));
    }

    public void UpdatePosition(GpsPosition position)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));
        CheckRule(new InstallationMustHaveValidGpsPosition(position));

        Position = position;
        QualityGrade = position.CalculateQualityGrade();

        AddDomainEvent(new InstallationUpdated(Id, DateTime.UtcNow));
    }

    public void UpdateDescription(Description? description)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        Description = description;

        AddDomainEvent(new InstallationUpdated(Id, DateTime.UtcNow));
    }

    public void UpdateCableSpec(CableSpec? cableSpec)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        CableSpec = cableSpec;

        AddDomainEvent(new InstallationUpdated(Id, DateTime.UtcNow));
    }

    public void UpdateDepth(Depth? depth)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        Depth = depth;

        AddDomainEvent(new InstallationUpdated(Id, DateTime.UtcNow));
    }

    public void MarkAsCompleted()
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        Status = InstallationStatus.Completed;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new InstallationCompleted(Id, DateTime.UtcNow));
    }

    public void Delete()
    {
        AddDomainEvent(new InstallationDeleted(Id, ProjectId, DateTime.UtcNow));
    }

    public void UpdateDeviceInfo(Manufacturer? manufacturer, ModelName? modelName, SerialNumber? serialNumber)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        Manufacturer = manufacturer;
        ModelName = modelName;
        SerialNumber = serialNumber;

        AddDomainEvent(new InstallationUpdated(Id, DateTime.UtcNow));
    }
}

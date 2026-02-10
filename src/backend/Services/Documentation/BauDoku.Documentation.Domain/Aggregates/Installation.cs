using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.Entities;
using BauDoku.Documentation.Domain.Events;
using BauDoku.Documentation.Domain.Rules;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Aggregates;

public sealed class Installation : AggregateRoot<InstallationId>
{
    private readonly List<Photo> _photos = [];
    private readonly List<Measurement> _measurements = [];

    public Guid ProjectId { get; private set; }
    public Guid? ZoneId { get; private set; }
    public InstallationType Type { get; private set; } = default!;
    public InstallationStatus Status { get; private set; } = default!;
    public GpsPosition Position { get; private set; } = default!;
    public Description? Description { get; private set; }
    public CableSpec? CableSpec { get; private set; }
    public Depth? Depth { get; private set; }
    public Manufacturer? Manufacturer { get; private set; }
    public ModelName? ModelName { get; private set; }
    public SerialNumber? SerialNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public IReadOnlyList<Photo> Photos => _photos.AsReadOnly();
    public IReadOnlyList<Measurement> Measurements => _measurements.AsReadOnly();

    private Installation() { }

    public static Installation Create(
        InstallationId id,
        Guid projectId,
        Guid? zoneId,
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

        var installation = new Installation
        {
            Id = id,
            ProjectId = projectId,
            ZoneId = zoneId,
            Type = type,
            Status = InstallationStatus.InProgress,
            Position = position,
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

        return installation;
    }

    public void AddPhoto(
        PhotoId photoId,
        string fileName,
        string blobUrl,
        string contentType,
        long fileSize,
        PhotoType photoType,
        Caption? caption = null,
        Description? description = null,
        GpsPosition? position = null)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        var photo = Photo.Create(
            photoId, fileName, blobUrl, contentType, fileSize,
            photoType, caption, description, position);

        _photos.Add(photo);

        AddDomainEvent(new PhotoAdded(Id, photoId, DateTime.UtcNow));
    }

    public void RemovePhoto(PhotoId photoId)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        var photo = _photos.FirstOrDefault(p => p.Id == photoId)
            ?? throw new InvalidOperationException($"Foto mit ID {photoId.Value} nicht gefunden.");

        _photos.Remove(photo);

        AddDomainEvent(new PhotoRemoved(Id, photoId, DateTime.UtcNow));
    }

    public void RecordMeasurement(
        MeasurementId measurementId,
        MeasurementType type,
        MeasurementValue value,
        string? notes = null)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));
        CheckRule(new MeasurementValueMustBePositive(value));

        var measurement = Measurement.Create(measurementId, type, value, notes);
        _measurements.Add(measurement);

        AddDomainEvent(new MeasurementRecorded(Id, measurementId, DateTime.UtcNow));
    }

    public void RemoveMeasurement(MeasurementId measurementId)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        var measurement = _measurements.FirstOrDefault(m => m.Id == measurementId)
            ?? throw new InvalidOperationException($"Messung mit ID {measurementId.Value} nicht gefunden.");

        _measurements.Remove(measurement);

        AddDomainEvent(new MeasurementRemoved(Id, measurementId, DateTime.UtcNow));
    }

    public void MarkAsCompleted()
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        Status = InstallationStatus.Completed;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new InstallationCompleted(Id, DateTime.UtcNow));
    }
}

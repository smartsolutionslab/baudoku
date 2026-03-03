using System.Collections.Frozen;
using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed class Installation : EventSourcedAggregateRoot<InstallationIdentifier>
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
    public bool IsDeleted { get; private set; }
    public IReadOnlyList<Photo> Photos => photos.AsReadOnly();
    public IReadOnlyList<Measurement> Measurements => measurements.AsReadOnly();

    public Installation() { }

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
        var now = DateTime.UtcNow;

        Installation installation = new();

        InstallationDocumented @event = new(
            id,
            projectId,
            zoneId,
            type,
            InstallationStatus.InProgress,
            position.Latitude,
            position.Longitude,
            position.Altitude,
            position.HorizontalAccuracy,
            position.Source,
            position.CorrectionService,
            position.RtkFixStatus,
            position.SatelliteCount,
            position.Hdop,
            position.CorrectionAge,
            qualityGrade,
            description,
            cableSpec?.CableType,
            cableSpec?.CrossSection,
            cableSpec?.Color,
            cableSpec?.ConductorCount,
            depth?.ValueInMillimeters,
            manufacturer,
            modelName,
            serialNumber,
            now,
            now);
        installation.RaiseEvent(@event);

        if (qualityGrade == GpsQualityGrade.D)
        {
            installation.RaiseEvent(new LowGpsQualityDetected(id, qualityGrade, now));
        }

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

        var actualTakenAt = takenAt ?? DateTime.UtcNow;

        PhotoAdded @event = new(
            Id,
            photoId,
            fileName,
            blobUrl,
            contentType,
            fileSize,
            photoType,
            caption,
            description,
            position?.Latitude,
            position?.Longitude,
            position?.Altitude,
            position?.HorizontalAccuracy,
            position?.Source,
            position?.CorrectionService,
            position?.RtkFixStatus,
            position?.SatelliteCount,
            position?.Hdop,
            position?.CorrectionAge,
            actualTakenAt,
            DateTime.UtcNow);
        RaiseEvent(@event);
    }

    public void RemovePhoto(PhotoIdentifier photoId)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        _ = photos.FirstOrDefault(p => p.Id == photoId) ?? throw new InvalidOperationException($"Foto mit ID {photoId.Value} nicht gefunden.");

        RaiseEvent(new PhotoRemoved(Id, photoId, DateTime.UtcNow));
    }

    public void RecordMeasurement(MeasurementIdentifier measurementId, MeasurementType type, MeasurementValue value, Notes? notes = null)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));
        CheckRule(new MeasurementValueMustBeNonNegative(value));
        CheckRule(new MeasurementTypeMustMatchInstallationType(Type, type));

        var result = Measurement.Evaluate(value);
        var now = DateTime.UtcNow;

        MeasurementRecorded @event = new(
            Id,
            measurementId,
            type,
            value.Value,
            value.Unit,
            value.MinThreshold,
            value.MaxThreshold,
            result,
            notes,
            now,
            now);
        RaiseEvent(@event);
    }

    public void RemoveMeasurement(MeasurementIdentifier measurementId)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        _ = measurements.FirstOrDefault(m => m.Id == measurementId) ?? throw new InvalidOperationException($"Messung mit ID {measurementId.Value} nicht gefunden.");

        RaiseEvent(new MeasurementRemoved(Id, measurementId, DateTime.UtcNow));
    }

    public void UpdatePosition(GpsPosition position)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));
        CheckRule(new InstallationMustHaveValidGpsPosition(position));

        var qualityGrade = position.CalculateQualityGrade();

        InstallationPositionUpdated @event = new(
            Id,
            position.Latitude,
            position.Longitude,
            position.Altitude,
            position.HorizontalAccuracy,
            position.Source,
            position.CorrectionService,
            position.RtkFixStatus,
            position.SatelliteCount,
            position.Hdop,
            position.CorrectionAge,
            qualityGrade,
            DateTime.UtcNow);
        RaiseEvent(@event);
    }

    public void UpdateDescription(Description? description)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        RaiseEvent(new InstallationDescriptionUpdated(Id, description, DateTime.UtcNow));
    }

    public void UpdateCableSpec(CableSpec? cableSpec)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        InstallationCableSpecUpdated @event = new(
            Id,
            cableSpec?.CableType,
            cableSpec?.CrossSection,
            cableSpec?.Color,
            cableSpec?.ConductorCount,
            DateTime.UtcNow);
        RaiseEvent(@event);
    }

    public void UpdateDepth(Depth? depth)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        RaiseEvent(new InstallationDepthUpdated(Id, depth?.ValueInMillimeters, DateTime.UtcNow));
    }

    public void MarkAsCompleted()
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        var now = DateTime.UtcNow;
        RaiseEvent(new InstallationCompleted(Id, now, now));
    }

    public void Delete()
    {
        RaiseEvent(new InstallationDeleted(Id, ProjectId, DateTime.UtcNow));
    }

    public void UpdateDeviceInfo(Manufacturer? manufacturer, ModelName? modelName, SerialNumber? serialNumber)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        RaiseEvent(new InstallationDeviceInfoUpdated(Id, manufacturer, modelName, serialNumber, DateTime.UtcNow));
    }

    // --- Event Application (state reconstitution) ---

    private static readonly FrozenDictionary<Type, Action<Installation, IDomainEvent>> eventAppliers =
        new Dictionary<Type, Action<Installation, IDomainEvent>>
        {
            [typeof(InstallationDocumented)] = Dispatch<InstallationDocumented>((i, @event) => i.Apply(@event)),
            [typeof(PhotoAdded)] = Dispatch<PhotoAdded>((i, @event) => i.Apply(@event)),
            [typeof(PhotoRemoved)] = Dispatch<PhotoRemoved>((i, @event) => i.Apply(@event)),
            [typeof(MeasurementRecorded)] = Dispatch<MeasurementRecorded>((i, @event) => i.Apply(@event)),
            [typeof(MeasurementRemoved)] = Dispatch<MeasurementRemoved>((i, @event) => i.Apply(@event)),
            [typeof(InstallationPositionUpdated)] = Dispatch<InstallationPositionUpdated>((i, @event) => i.Apply(@event)),
            [typeof(InstallationDescriptionUpdated)] = Dispatch<InstallationDescriptionUpdated>((i, @event) => i.Apply(@event)),
            [typeof(InstallationCableSpecUpdated)] = Dispatch<InstallationCableSpecUpdated>((i, @event) => i.Apply(@event)),
            [typeof(InstallationDepthUpdated)] = Dispatch<InstallationDepthUpdated>((i, @event) => i.Apply(@event)),
            [typeof(InstallationDeviceInfoUpdated)] = Dispatch<InstallationDeviceInfoUpdated>((i, @event) => i.Apply(@event)),
            [typeof(InstallationCompleted)] = Dispatch<InstallationCompleted>((i, @event) => i.Apply(@event)),
            [typeof(InstallationDeleted)] = Dispatch<InstallationDeleted>((i, @event) => i.Apply(@event)),
        }.ToFrozenDictionary();

    private static Action<Installation, IDomainEvent> Dispatch<TEvent>(Action<Installation, TEvent> handler) where TEvent : IDomainEvent =>
        (i, @event) => handler(i, (TEvent)@event);

    public override void Apply(IDomainEvent @event)
    {
        if (eventAppliers.TryGetValue(@event.GetType(), out var applier))
        {
            applier(this, @event);
        }
    }

    public void Apply(InstallationDocumented @event)
    {
        Id = @event.InstallationId;
        ProjectId = @event.ProjectId;
        ZoneId = @event.ZoneId;
        Type = @event.Type;
        Status = @event.Status;
        Position = ReconstructPosition(@event.Latitude, @event.Longitude, @event.Altitude, @event.HorizontalAccuracy, @event.GpsSource, @event.CorrectionService, @event.RtkFixStatus, @event.SatelliteCount, @event.Hdop, @event.CorrectionAge);
        QualityGrade = @event.QualityGrade;
        Description = @event.Description;
        CableSpec = ReconstructCableSpec(@event.CableType, @event.CrossSection, @event.CableColor, @event.ConductorCount);
        Depth = Depth.FromNullable(@event.DepthMm);
        Manufacturer = @event.Manufacturer;
        ModelName = @event.ModelName;
        SerialNumber = @event.SerialNumber;
        CreatedAt = @event.CreatedAt;
    }

    public void Apply(PhotoAdded @event)
    {
        var position = ReconstructNullablePosition(@event.Latitude, @event.Longitude, @event.Altitude, @event.HorizontalAccuracy, @event.GpsSource, @event.CorrectionService, @event.RtkFixStatus, @event.SatelliteCount, @event.Hdop, @event.CorrectionAge);

        var photo = Photo.Reconstitute(
            @event.PhotoId,
            @event.FileName,
            @event.BlobUrl,
            @event.ContentType,
            @event.FileSize,
            @event.PhotoType,
            @event.Caption,
            @event.Description,
            position,
            @event.TakenAt);

        photos.Add(photo);
    }

    public void Apply(PhotoRemoved @event)
    {
        var photo = photos.FirstOrDefault(p => p.Id == @event.PhotoId);
        if (photo is not null)
        {
            photos.Remove(photo);
        }
    }

    public void Apply(MeasurementRecorded @event)
    {
        var measurement = Measurement.Reconstitute(
            @event.MeasurementId,
            @event.Type,
            MeasurementValue.Create(@event.Value, @event.Unit, @event.MinThreshold, @event.MaxThreshold),
            @event.Result,
            @event.Notes,
            @event.MeasuredAt);

        measurements.Add(measurement);
    }

    public void Apply(MeasurementRemoved @event)
    {
        var measurement = measurements.FirstOrDefault(m => m.Id == @event.MeasurementId);
        if (measurement is not null)
        {
            measurements.Remove(measurement);
        }
    }

    public void Apply(InstallationPositionUpdated @event)
    {
        Position = ReconstructPosition(
            @event.Latitude,
            @event.Longitude,
            @event.Altitude,
            @event.HorizontalAccuracy,
            @event.GpsSource,
            @event.CorrectionService,
            @event.RtkFixStatus,
            @event.SatelliteCount,
            @event.Hdop,
            @event.CorrectionAge);
        QualityGrade = @event.QualityGrade;
    }

    public void Apply(InstallationDescriptionUpdated @event)
    {
        Description = @event.Description;
    }

    public void Apply(InstallationCableSpecUpdated @event)
    {
        CableSpec = ReconstructCableSpec(@event.CableType, @event.CrossSection, @event.CableColor, @event.ConductorCount);
    }

    public void Apply(InstallationDepthUpdated @event)
    {
        Depth = Depth.FromNullable(@event.DepthMm);
    }

    public void Apply(InstallationDeviceInfoUpdated @event)
    {
        Manufacturer = @event.Manufacturer;
        ModelName = @event.ModelName;
        SerialNumber = @event.SerialNumber;
    }

    public void Apply(InstallationCompleted @event)
    {
        Status = InstallationStatus.Completed;
        CompletedAt = @event.CompletedAt;
    }

    public void Apply(InstallationDeleted @event)
    {
        IsDeleted = true;
    }

    // --- Reconstruction helpers ---

    private static GpsPosition ReconstructPosition(
        Latitude latitude, Longitude longitude, Altitude? altitude, HorizontalAccuracy horizontalAccuracy, GpsSource gpsSource,
        CorrectionService? correctionService, RtkFixStatus? rtkFixStatus, SatelliteCount? satelliteCount, Hdop? hdop, CorrectionAge? correctionAge)
    {
        return GpsPosition.Create(
            latitude, longitude, altitude,
            horizontalAccuracy, gpsSource,
            correctionService, rtkFixStatus,
            satelliteCount, hdop, correctionAge);
    }

    private static GpsPosition? ReconstructNullablePosition(
        Latitude? latitude, Longitude? longitude, Altitude? altitude, HorizontalAccuracy? horizontalAccuracy, GpsSource? gpsSource,
        CorrectionService? correctionService, RtkFixStatus? rtkFixStatus, SatelliteCount? satelliteCount, Hdop? hdop, CorrectionAge? correctionAge)
    {
        if (latitude is null) return null;
        return ReconstructPosition(
            latitude, longitude!, altitude, horizontalAccuracy!, gpsSource!,
            correctionService, rtkFixStatus, satelliteCount, hdop, correctionAge);
    }

    private static CableSpec? ReconstructCableSpec(CableType? cableType, CrossSection? crossSection, CableColor? cableColor, ConductorCount? conductorCount)
        => CableSpec.FromNullable(cableType, crossSection, cableColor, conductorCount);
}

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

        var installation = new Installation();

        installation.RaiseEvent(new InstallationDocumented(
            id.Value,
            projectId.Value,
            zoneId?.Value,
            type.Value,
            InstallationStatus.InProgress.Value,
            position.Latitude.Value,
            position.Longitude.Value,
            position.Altitude,
            position.HorizontalAccuracy.Value,
            position.Source.Value,
            position.CorrectionService?.Value,
            position.RtkFixStatus?.Value,
            position.SatelliteCount,
            position.Hdop,
            position.CorrectionAge,
            qualityGrade.Value,
            description?.Value,
            cableSpec?.CableType.Value,
            cableSpec?.CrossSection?.Value,
            cableSpec?.Color?.Value,
            cableSpec?.ConductorCount,
            depth?.ValueInMillimeters,
            manufacturer?.Value,
            modelName?.Value,
            serialNumber?.Value,
            now,
            now));

        if (qualityGrade == GpsQualityGrade.D)
            installation.RaiseEvent(new LowGpsQualityDetected(id.Value, qualityGrade.Value, now));

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

        RaiseEvent(new PhotoAdded(
            Id.Value,
            photoId.Value,
            fileName.Value,
            blobUrl.Value,
            contentType.Value,
            fileSize.Value,
            photoType.Value,
            caption?.Value,
            description?.Value,
            position?.Latitude.Value,
            position?.Longitude.Value,
            position?.Altitude,
            position?.HorizontalAccuracy.Value,
            position?.Source.Value,
            position?.CorrectionService?.Value,
            position?.RtkFixStatus?.Value,
            position?.SatelliteCount,
            position?.Hdop,
            position?.CorrectionAge,
            actualTakenAt,
            DateTime.UtcNow));
    }

    public void RemovePhoto(PhotoIdentifier photoId)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        _ = photos.FirstOrDefault(p => p.Id == photoId)
            ?? throw new InvalidOperationException($"Foto mit ID {photoId.Value} nicht gefunden.");

        RaiseEvent(new PhotoRemoved(Id.Value, photoId.Value, DateTime.UtcNow));
    }

    public void RecordMeasurement(MeasurementIdentifier measurementId, MeasurementType type, MeasurementValue value, Notes? notes = null)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));
        CheckRule(new MeasurementValueMustBeNonNegative(value));
        CheckRule(new MeasurementTypeMustMatchInstallationType(Type, type));

        var result = Measurement.Evaluate(value);
        var now = DateTime.UtcNow;

        RaiseEvent(new MeasurementRecorded(
            Id.Value,
            measurementId.Value,
            type.Value,
            value.Value,
            value.Unit.Value,
            value.MinThreshold,
            value.MaxThreshold,
            result.Value,
            notes?.Value,
            now,
            now));
    }

    public void RemoveMeasurement(MeasurementIdentifier measurementId)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        _ = measurements.FirstOrDefault(m => m.Id == measurementId)
            ?? throw new InvalidOperationException($"Messung mit ID {measurementId.Value} nicht gefunden.");

        RaiseEvent(new MeasurementRemoved(Id.Value, measurementId.Value, DateTime.UtcNow));
    }

    public void UpdatePosition(GpsPosition position)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));
        CheckRule(new InstallationMustHaveValidGpsPosition(position));

        var qualityGrade = position.CalculateQualityGrade();

        RaiseEvent(new InstallationPositionUpdated(
            Id.Value,
            position.Latitude.Value,
            position.Longitude.Value,
            position.Altitude,
            position.HorizontalAccuracy.Value,
            position.Source.Value,
            position.CorrectionService?.Value,
            position.RtkFixStatus?.Value,
            position.SatelliteCount,
            position.Hdop,
            position.CorrectionAge,
            qualityGrade.Value,
            DateTime.UtcNow));
    }

    public void UpdateDescription(Description? description)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        RaiseEvent(new InstallationDescriptionUpdated(Id.Value, description?.Value, DateTime.UtcNow));
    }

    public void UpdateCableSpec(CableSpec? cableSpec)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        RaiseEvent(new InstallationCableSpecUpdated(
            Id.Value,
            cableSpec?.CableType.Value,
            cableSpec?.CrossSection?.Value,
            cableSpec?.Color?.Value,
            cableSpec?.ConductorCount,
            DateTime.UtcNow));
    }

    public void UpdateDepth(Depth? depth)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        RaiseEvent(new InstallationDepthUpdated(Id.Value, depth?.ValueInMillimeters, DateTime.UtcNow));
    }

    public void MarkAsCompleted()
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        var now = DateTime.UtcNow;
        RaiseEvent(new InstallationCompleted(Id.Value, now, now));
    }

    public void Delete()
    {
        RaiseEvent(new InstallationDeleted(Id.Value, ProjectId.Value, DateTime.UtcNow));
    }

    public void UpdateDeviceInfo(Manufacturer? manufacturer, ModelName? modelName, SerialNumber? serialNumber)
    {
        CheckRule(new CompletedInstallationCannotBeModified(Status));

        RaiseEvent(new InstallationDeviceInfoUpdated(
            Id.Value, manufacturer?.Value, modelName?.Value, serialNumber?.Value, DateTime.UtcNow));
    }

    // --- Event Application (state reconstitution) ---

    public override void Apply(IDomainEvent @event)
    {
        switch (@event)
        {
            case InstallationDocumented e: Apply(e); break;
            case PhotoAdded e: Apply(e); break;
            case PhotoRemoved e: Apply(e); break;
            case MeasurementRecorded e: Apply(e); break;
            case MeasurementRemoved e: Apply(e); break;
            case InstallationPositionUpdated e: Apply(e); break;
            case InstallationDescriptionUpdated e: Apply(e); break;
            case InstallationCableSpecUpdated e: Apply(e); break;
            case InstallationDepthUpdated e: Apply(e); break;
            case InstallationDeviceInfoUpdated e: Apply(e); break;
            case InstallationCompleted e: Apply(e); break;
            case InstallationDeleted e: Apply(e); break;
            case LowGpsQualityDetected: break; // notification only, no state change
        }
    }

    public void Apply(InstallationDocumented e)
    {
        Id = InstallationIdentifier.From(e.InstallationId);
        ProjectId = ProjectIdentifier.From(e.ProjectId);
        ZoneId = ZoneIdentifier.FromNullable(e.ZoneId);
        Type = InstallationType.From(e.Type);
        Status = InstallationStatus.From(e.Status);
        Position = GpsPosition.Create(
            Latitude.From(e.Latitude),
            Longitude.From(e.Longitude),
            e.Altitude,
            HorizontalAccuracy.From(e.HorizontalAccuracy),
            GpsSource.From(e.GpsSource),
            CorrectionService.FromNullable(e.CorrectionService),
            RtkFixStatus.FromNullable(e.RtkFixStatus),
            e.SatelliteCount,
            e.Hdop,
            e.CorrectionAge);
        QualityGrade = GpsQualityGrade.From(e.QualityGrade);
        Description = Description.FromNullable(e.Description);
        CableSpec = e.CableType is not null
            ? CableSpec.Create(
                CableType.From(e.CableType),
                CrossSection.FromNullable(e.CrossSection),
                CableColor.FromNullable(e.CableColor),
                e.ConductorCount)
            : null;
        Depth = Depth.FromNullable(e.DepthMm);
        Manufacturer = Manufacturer.FromNullable(e.Manufacturer);
        ModelName = ModelName.FromNullable(e.ModelName);
        SerialNumber = SerialNumber.FromNullable(e.SerialNumber);
        CreatedAt = e.CreatedAt;
    }

    public void Apply(PhotoAdded e)
    {
        GpsPosition? position = e.Latitude.HasValue
            ? GpsPosition.Create(
                Latitude.From(e.Latitude.Value),
                Longitude.From(e.Longitude!.Value),
                e.Altitude,
                HorizontalAccuracy.From(e.HorizontalAccuracy!.Value),
                GpsSource.From(e.GpsSource!),
                CorrectionService.FromNullable(e.CorrectionService),
                RtkFixStatus.FromNullable(e.RtkFixStatus),
                e.SatelliteCount,
                e.Hdop,
                e.CorrectionAge)
            : null;

        var photo = Photo.Reconstitute(
            PhotoIdentifier.From(e.PhotoId),
            FileName.From(e.FileName),
            BlobUrl.From(e.BlobUrl),
            ContentType.From(e.ContentType),
            FileSize.From(e.FileSize),
            PhotoType.From(e.PhotoType),
            Caption.FromNullable(e.Caption),
            Description.FromNullable(e.Description),
            position,
            e.TakenAt);

        photos.Add(photo);
    }

    public void Apply(PhotoRemoved e)
    {
        var photo = photos.FirstOrDefault(p => p.Id.Value == e.PhotoId);
        if (photo is not null)
            photos.Remove(photo);
    }

    public void Apply(MeasurementRecorded e)
    {
        var measurement = Measurement.Reconstitute(
            MeasurementIdentifier.From(e.MeasurementId),
            MeasurementType.From(e.Type),
            MeasurementValue.Create(e.Value, e.Unit, e.MinThreshold, e.MaxThreshold),
            MeasurementResult.From(e.Result),
            Notes.FromNullable(e.Notes),
            e.MeasuredAt);

        measurements.Add(measurement);
    }

    public void Apply(MeasurementRemoved e)
    {
        var measurement = measurements.FirstOrDefault(m => m.Id.Value == e.MeasurementId);
        if (measurement is not null)
            measurements.Remove(measurement);
    }

    public void Apply(InstallationPositionUpdated e)
    {
        Position = GpsPosition.Create(
            Latitude.From(e.Latitude),
            Longitude.From(e.Longitude),
            e.Altitude,
            HorizontalAccuracy.From(e.HorizontalAccuracy),
            GpsSource.From(e.GpsSource),
            CorrectionService.FromNullable(e.CorrectionService),
            RtkFixStatus.FromNullable(e.RtkFixStatus),
            e.SatelliteCount,
            e.Hdop,
            e.CorrectionAge);
        QualityGrade = GpsQualityGrade.From(e.QualityGrade);
    }

    public void Apply(InstallationDescriptionUpdated e)
    {
        Description = Description.FromNullable(e.Description);
    }

    public void Apply(InstallationCableSpecUpdated e)
    {
        CableSpec = e.CableType is not null
            ? CableSpec.Create(
                CableType.From(e.CableType),
                CrossSection.FromNullable(e.CrossSection),
                CableColor.FromNullable(e.CableColor),
                e.ConductorCount)
            : null;
    }

    public void Apply(InstallationDepthUpdated e)
    {
        Depth = Depth.FromNullable(e.DepthMm);
    }

    public void Apply(InstallationDeviceInfoUpdated e)
    {
        Manufacturer = Manufacturer.FromNullable(e.Manufacturer);
        ModelName = ModelName.FromNullable(e.ModelName);
        SerialNumber = SerialNumber.FromNullable(e.SerialNumber);
    }

    public void Apply(InstallationCompleted e)
    {
        Status = InstallationStatus.Completed;
        CompletedAt = e.CompletedAt;
    }

    public void Apply(InstallationDeleted e)
    {
        IsDeleted = true;
    }
}

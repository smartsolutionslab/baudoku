using System.Collections.Frozen;
using SmartSolutionsLab.BauDoku.Documentation.Domain;
using SmartSolutionsLab.BauDoku.Documentation.Infrastructure.ReadModel;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace SmartSolutionsLab.BauDoku.Documentation.Infrastructure.Projections;

public sealed class InstallationReadModelProjection(IServiceScopeFactory scopeFactory) : IProjection
{
    private delegate Task EventHandler(ReadModelDbContext dbContext, object eventData);

    private static readonly FrozenDictionary<Type, EventHandler> eventHandlers = new Dictionary<Type, EventHandler>
    {
        [typeof(InstallationDocumented)] = Dispatch<InstallationDocumented>(ApplyInstallationDocumented),
        [typeof(PhotoAdded)] = Dispatch<PhotoAdded>(ApplyPhotoAdded),
        [typeof(PhotoRemoved)] = Dispatch<PhotoRemoved>(ApplyPhotoRemoved),
        [typeof(MeasurementRecorded)] = Dispatch<MeasurementRecorded>(ApplyMeasurementRecorded),
        [typeof(MeasurementRemoved)] = Dispatch<MeasurementRemoved>(ApplyMeasurementRemoved),
        [typeof(InstallationPositionUpdated)] = Dispatch<InstallationPositionUpdated>(ApplyPositionUpdated),
        [typeof(InstallationDescriptionUpdated)] = Dispatch<InstallationDescriptionUpdated>(ApplyDescriptionUpdated),
        [typeof(InstallationCableSpecUpdated)] = Dispatch<InstallationCableSpecUpdated>(ApplyCableSpecUpdated),
        [typeof(InstallationDepthUpdated)] = Dispatch<InstallationDepthUpdated>(ApplyDepthUpdated),
        [typeof(InstallationDeviceInfoUpdated)] = Dispatch<InstallationDeviceInfoUpdated>(ApplyDeviceInfoUpdated),
        [typeof(InstallationCompleted)] = Dispatch<InstallationCompleted>(ApplyCompleted),
        [typeof(InstallationDeleted)] = Dispatch<InstallationDeleted>(ApplyDeleted),
    }.ToFrozenDictionary();

    private static EventHandler Dispatch<TEvent>(Func<ReadModelDbContext, TEvent, Task> handler) => (db, @event) => handler(db, (TEvent)@event);

    public void Apply(IDocumentOperations operations, IReadOnlyList<IEvent> events) => throw new NotSupportedException("Use async projection only.");

    public async Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<IEvent> events, CancellationToken cancellation)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReadModelDbContext>();

        foreach (var @event in events)
        {
            var eventType = @event.Data.GetType();
            if (eventHandlers.TryGetValue(eventType, out var handler))
            {
                await handler(dbContext, @event.Data);
            }
        }

        await dbContext.SaveChangesAsync(cancellation);
    }

    private static async Task UpdateInstallation(ReadModelDbContext dbContext, Guid installationId, Action<InstallationReadModel> apply)
    {
        var installation = await dbContext.Installations.FindAsync(installationId);
        if (installation is null) return;

        apply(installation);
    }

    private static Task ApplyInstallationDocumented(ReadModelDbContext dbContext, InstallationDocumented @event)
    {
        dbContext.Installations.Add(new InstallationReadModel
        {
            Id = @event.InstallationId.Value,
            ProjectId = @event.ProjectId.Value,
            ZoneId = @event.ZoneId?.Value,
            Type = @event.Type.Value,
            Status = @event.Status.Value,
            Latitude = @event.Latitude?.Value,
            Longitude = @event.Longitude?.Value,
            Altitude = @event.Altitude?.Value,
            HorizontalAccuracy = @event.HorizontalAccuracy?.Value,
            GpsSource = @event.GpsSource?.Value,
            CorrectionService = @event.CorrectionService?.Value,
            RtkFixStatus = @event.RtkFixStatus?.Value,
            SatelliteCount = @event.SatelliteCount?.Value,
            Hdop = @event.Hdop?.Value,
            CorrectionAge = @event.CorrectionAge?.Value,
            QualityGrade = @event.QualityGrade?.Value,
            Description = @event.Description?.Value,
            CableType = @event.CableType?.Value,
            CrossSection = @event.CrossSection?.Value,
            CableColor = @event.CableColor?.Value,
            ConductorCount = @event.ConductorCount?.Value,
            DepthMm = @event.Depth?.ValueInMillimeters,
            Manufacturer = @event.Manufacturer?.Value,
            ModelName = @event.ModelName?.Value,
            SerialNumber = @event.SerialNumber?.Value,
            CreatedAt = @event.CreatedAt,
            PhotoCount = 0,
            MeasurementCount = 0,
            IsDeleted = false
        });

        return Task.CompletedTask;
    }

    private static async Task ApplyPhotoAdded(ReadModelDbContext dbContext, PhotoAdded @event)
    {
        dbContext.Photos.Add(new PhotoReadModel
        {
            Id = @event.PhotoId.Value,
            InstallationId = @event.InstallationId.Value,
            FileName = @event.FileName.Value,
            BlobUrl = @event.BlobUrl.Value,
            ContentType = @event.ContentType.Value,
            FileSize = @event.FileSize.Value,
            PhotoType = @event.PhotoType.Value,
            Caption = @event.Caption?.Value,
            Description = @event.Description?.Value,
            Latitude = @event.Latitude?.Value,
            Longitude = @event.Longitude?.Value,
            Altitude = @event.Altitude?.Value,
            HorizontalAccuracy = @event.HorizontalAccuracy?.Value,
            GpsSource = @event.GpsSource?.Value,
            CorrectionService = @event.CorrectionService?.Value,
            RtkFixStatus = @event.RtkFixStatus?.Value,
            SatelliteCount = @event.SatelliteCount?.Value,
            Hdop = @event.Hdop?.Value,
            CorrectionAge = @event.CorrectionAge?.Value,
            TakenAt = @event.TakenAt
        });

        await UpdateInstallation(dbContext, @event.InstallationId.Value, i => i.PhotoCount++);
    }

    private static async Task ApplyPhotoRemoved(ReadModelDbContext dbContext, PhotoRemoved @event)
    {
        var photo = await dbContext.Photos.FindAsync(@event.PhotoId.Value);
        if (photo is not null)
        {
            dbContext.Photos.Remove(photo);
        }

        await UpdateInstallation(dbContext, @event.InstallationId.Value, i => i.PhotoCount--);
    }

    private static async Task ApplyMeasurementRecorded(ReadModelDbContext dbContext, MeasurementRecorded @event)
    {
        dbContext.Measurements.Add(new MeasurementReadModel
        {
            Id = @event.MeasurementId.Value,
            InstallationId = @event.InstallationId.Value,
            Type = @event.Type.Value,
            Value = @event.Value,
            Unit = @event.Unit.Value,
            MinThreshold = @event.MinThreshold,
            MaxThreshold = @event.MaxThreshold,
            Result = @event.Result.Value,
            Notes = @event.Notes?.Value,
            MeasuredAt = @event.MeasuredAt
        });

        await UpdateInstallation(dbContext, @event.InstallationId.Value, i => i.MeasurementCount++);
    }

    private static async Task ApplyMeasurementRemoved(ReadModelDbContext dbContext, MeasurementRemoved @event)
    {
        var measurement = await dbContext.Measurements.FindAsync(@event.MeasurementId.Value);
        if (measurement is not null)
        {
            dbContext.Measurements.Remove(measurement);
        }

        await UpdateInstallation(dbContext, @event.InstallationId.Value, i => i.MeasurementCount--);
    }

    private static Task ApplyPositionUpdated(ReadModelDbContext dbContext, InstallationPositionUpdated @event) =>
        UpdateInstallation(dbContext, @event.InstallationId.Value, i =>
        {
            i.Latitude = @event.Latitude.Value;
            i.Longitude = @event.Longitude.Value;
            i.Altitude = @event.Altitude?.Value;
            i.HorizontalAccuracy = @event.HorizontalAccuracy.Value;
            i.GpsSource = @event.GpsSource.Value;
            i.CorrectionService = @event.CorrectionService?.Value;
            i.RtkFixStatus = @event.RtkFixStatus?.Value;
            i.SatelliteCount = @event.SatelliteCount?.Value;
            i.Hdop = @event.Hdop?.Value;
            i.CorrectionAge = @event.CorrectionAge?.Value;
            i.QualityGrade = @event.QualityGrade.Value;
        });

    private static Task ApplyDescriptionUpdated(ReadModelDbContext dbContext, InstallationDescriptionUpdated @event) =>
        UpdateInstallation(dbContext, @event.InstallationId.Value, i => i.Description = @event.Description?.Value);

    private static Task ApplyCableSpecUpdated(ReadModelDbContext dbContext, InstallationCableSpecUpdated @event) =>
        UpdateInstallation(dbContext, @event.InstallationId.Value, i =>
        {
            i.CableType = @event.CableType?.Value;
            i.CrossSection = @event.CrossSection?.Value;
            i.CableColor = @event.CableColor?.Value;
            i.ConductorCount = @event.ConductorCount?.Value;
        });

    private static Task ApplyDepthUpdated(ReadModelDbContext dbContext, InstallationDepthUpdated @event) =>
        UpdateInstallation(dbContext, @event.InstallationId.Value, i => i.DepthMm = @event.Depth?.ValueInMillimeters);

    private static Task ApplyDeviceInfoUpdated(ReadModelDbContext dbContext, InstallationDeviceInfoUpdated @event) =>
        UpdateInstallation(dbContext, @event.InstallationId.Value, i =>
        {
            i.Manufacturer = @event.Manufacturer?.Value;
            i.ModelName = @event.ModelName?.Value;
            i.SerialNumber = @event.SerialNumber?.Value;
        });

    private static Task ApplyCompleted(ReadModelDbContext dbContext, InstallationCompleted @event) =>
        UpdateInstallation(dbContext, @event.InstallationId.Value, i =>
        {
            i.Status = "completed";
            i.CompletedAt = @event.CompletedAt;
        });

    private static Task ApplyDeleted(ReadModelDbContext dbContext, InstallationDeleted @event) =>
        UpdateInstallation(dbContext, @event.InstallationId.Value, i => i.IsDeleted = true);
}

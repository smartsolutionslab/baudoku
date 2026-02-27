using System.Collections.Frozen;
using BauDoku.Documentation.Domain;
using BauDoku.Documentation.Infrastructure.ReadModel;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.Documentation.Infrastructure.Projections;

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

    private static EventHandler Dispatch<TEvent>(Func<ReadModelDbContext, TEvent, Task> handler) => (db, e) => handler(db, (TEvent)e);

    public void Apply(IDocumentOperations operations, IReadOnlyList<StreamAction> streams) => throw new NotSupportedException("Use async projection only.");

    public async Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<StreamAction> streams, CancellationToken cancellation)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReadModelDbContext>();

        foreach (var stream in streams)
        {
            foreach (var @event in stream.Events)
            {
                var eventType = @event.Data.GetType();
                if (eventHandlers.ContainsKey(eventType))
                {
                    EventHandler handler = eventHandlers[eventType];
                    await handler(dbContext, @event.Data);
                }
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
            Id = @event.InstallationId,
            ProjectId = @event.ProjectId,
            ZoneId = @event.ZoneId,
            Type = @event.Type,
            Status = @event.Status,
            Latitude = @event.Latitude,
            Longitude = @event.Longitude,
            Altitude = @event.Altitude,
            HorizontalAccuracy = @event.HorizontalAccuracy,
            GpsSource = @event.GpsSource,
            CorrectionService = @event.CorrectionService,
            RtkFixStatus = @event.RtkFixStatus,
            SatelliteCount = @event.SatelliteCount,
            Hdop = @event.Hdop,
            CorrectionAge = @event.CorrectionAge,
            QualityGrade = @event.QualityGrade,
            Description = @event.Description,
            CableType = @event.CableType,
            CrossSection = @event.CrossSection,
            CableColor = @event.CableColor,
            ConductorCount = @event.ConductorCount,
            DepthMm = @event.DepthMm,
            Manufacturer = @event.Manufacturer,
            ModelName = @event.ModelName,
            SerialNumber = @event.SerialNumber,
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
            Id = @event.PhotoId,
            InstallationId = @event.InstallationId,
            FileName = @event.FileName,
            BlobUrl = @event.BlobUrl,
            ContentType = @event.ContentType,
            FileSize = @event.FileSize,
            PhotoType = @event.PhotoType,
            Caption = @event.Caption,
            Description = @event.Description,
            Latitude = @event.Latitude,
            Longitude = @event.Longitude,
            Altitude = @event.Altitude,
            HorizontalAccuracy = @event.HorizontalAccuracy,
            GpsSource = @event.GpsSource,
            CorrectionService = @event.CorrectionService,
            RtkFixStatus = @event.RtkFixStatus,
            SatelliteCount = @event.SatelliteCount,
            Hdop = @event.Hdop,
            CorrectionAge = @event.CorrectionAge,
            TakenAt = @event.TakenAt
        });

        await UpdateInstallation(dbContext, @event.InstallationId, i => i.PhotoCount++);
    }

    private static async Task ApplyPhotoRemoved(ReadModelDbContext dbContext, PhotoRemoved @event)
    {
        var photo = await dbContext.Photos.FindAsync(@event.PhotoId);
        if (photo is not null)
        {
            dbContext.Photos.Remove(photo);
        }

        await UpdateInstallation(dbContext, @event.InstallationId, i => i.PhotoCount--);
    }

    private static async Task ApplyMeasurementRecorded(ReadModelDbContext dbContext, MeasurementRecorded e)
    {
        dbContext.Measurements.Add(new MeasurementReadModel
        {
            Id = e.MeasurementId,
            InstallationId = e.InstallationId,
            Type = e.Type,
            Value = e.Value,
            Unit = e.Unit,
            MinThreshold = e.MinThreshold,
            MaxThreshold = e.MaxThreshold,
            Result = e.Result,
            Notes = e.Notes,
            MeasuredAt = e.MeasuredAt
        });

        await UpdateInstallation(dbContext, e.InstallationId, i => i.MeasurementCount++);
    }

    private static async Task ApplyMeasurementRemoved(ReadModelDbContext dbContext, MeasurementRemoved @event)
    {
        var measurement = await dbContext.Measurements.FindAsync(@event.MeasurementId);
        if (measurement is not null)
        {
            dbContext.Measurements.Remove(measurement);
        }

        await UpdateInstallation(dbContext, @event.InstallationId, i => i.MeasurementCount--);
    }

    private static Task ApplyPositionUpdated(ReadModelDbContext dbContext, InstallationPositionUpdated @event) =>
        UpdateInstallation(dbContext, @event.InstallationId, i =>
        {
            i.Latitude = @event.Latitude;
            i.Longitude = @event.Longitude;
            i.Altitude = @event.Altitude;
            i.HorizontalAccuracy = @event.HorizontalAccuracy;
            i.GpsSource = @event.GpsSource;
            i.CorrectionService = @event.CorrectionService;
            i.RtkFixStatus = @event.RtkFixStatus;
            i.SatelliteCount = @event.SatelliteCount;
            i.Hdop = @event.Hdop;
            i.CorrectionAge = @event.CorrectionAge;
            i.QualityGrade = @event.QualityGrade;
        });

    private static Task ApplyDescriptionUpdated(ReadModelDbContext dbContext, InstallationDescriptionUpdated @event) =>
        UpdateInstallation(dbContext, @event.InstallationId, i => i.Description = @event.Description);

    private static Task ApplyCableSpecUpdated(ReadModelDbContext dbContext, InstallationCableSpecUpdated @event) =>
        UpdateInstallation(dbContext, @event.InstallationId, i =>
        {
            i.CableType = @event.CableType;
            i.CrossSection = @event.CrossSection;
            i.CableColor = @event.CableColor;
            i.ConductorCount = @event.ConductorCount;
        });

    private static Task ApplyDepthUpdated(ReadModelDbContext dbContext, InstallationDepthUpdated @event) =>
        UpdateInstallation(dbContext, @event.InstallationId, i => i.DepthMm = @event.DepthMm);

    private static Task ApplyDeviceInfoUpdated(ReadModelDbContext dbContext, InstallationDeviceInfoUpdated @event) =>
        UpdateInstallation(dbContext, @event.InstallationId, i =>
        {
            i.Manufacturer = @event.Manufacturer;
            i.ModelName = @event.ModelName;
            i.SerialNumber = @event.SerialNumber;
        });

    private static Task ApplyCompleted(ReadModelDbContext dbContext, InstallationCompleted @event) =>
        UpdateInstallation(dbContext, @event.InstallationId, i =>
        {
            i.Status = "completed";
            i.CompletedAt = @event.CompletedAt;
        });

    private static Task ApplyDeleted(ReadModelDbContext dbContext, InstallationDeleted @evet) =>
        UpdateInstallation(dbContext, evet.InstallationId, i => i.IsDeleted = true);
}

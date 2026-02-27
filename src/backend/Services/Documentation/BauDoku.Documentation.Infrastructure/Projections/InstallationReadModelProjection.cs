using BauDoku.Documentation.Domain;
using BauDoku.Documentation.Infrastructure.ReadModel;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.Documentation.Infrastructure.Projections;

public sealed class InstallationReadModelProjection(IServiceScopeFactory scopeFactory) : IProjection
{
    public void Apply(IDocumentOperations operations, IReadOnlyList<StreamAction> streams)
    {
        throw new NotSupportedException("Use async projection only.");
    }

    public async Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<StreamAction> streams, CancellationToken cancellation)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReadModelDbContext>();

        foreach (var stream in streams)
        {
            foreach (var @event in stream.Events)
            {
                switch (@event.Data)
                {
                    case InstallationDocumented e:
                        await HandleInstallationDocumented(dbContext, e);
                        break;
                    case PhotoAdded e:
                        await HandlePhotoAdded(dbContext, e);
                        break;
                    case PhotoRemoved e:
                        await HandlePhotoRemoved(dbContext, e);
                        break;
                    case MeasurementRecorded e:
                        await HandleMeasurementRecorded(dbContext, e);
                        break;
                    case MeasurementRemoved e:
                        await HandleMeasurementRemoved(dbContext, e);
                        break;
                    case InstallationPositionUpdated e:
                        await HandlePositionUpdated(dbContext, e);
                        break;
                    case InstallationDescriptionUpdated e:
                        await HandleDescriptionUpdated(dbContext, e);
                        break;
                    case InstallationCableSpecUpdated e:
                        await HandleCableSpecUpdated(dbContext, e);
                        break;
                    case InstallationDepthUpdated e:
                        await HandleDepthUpdated(dbContext, e);
                        break;
                    case InstallationDeviceInfoUpdated e:
                        await HandleDeviceInfoUpdated(dbContext, e);
                        break;
                    case InstallationCompleted e:
                        await HandleCompleted(dbContext, e);
                        break;
                    case InstallationDeleted e:
                        await HandleDeleted(dbContext, e);
                        break;
                    case LowGpsQualityDetected:
                        break; // notification only
                }
            }
        }

        await dbContext.SaveChangesAsync(cancellation);
    }

    private static Task HandleInstallationDocumented(ReadModelDbContext dbContext, InstallationDocumented @event)
    {
        InstallationReadModel model = new()
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
        };

        dbContext.Installations.Add(model);

        return Task.CompletedTask;
    }

    private static async Task HandlePhotoAdded(ReadModelDbContext dbContext, PhotoAdded @event)
    {
        PhotoReadModel photo = new()
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
        };

        dbContext.Photos.Add(photo);

        var installation = await dbContext.Installations.FindAsync(@event.InstallationId);
        if (installation is null) return;

        installation.PhotoCount++;
    }

    private static async Task HandlePhotoRemoved(ReadModelDbContext dbContext, PhotoRemoved @event)
    {
        var photo = await dbContext.Photos.FindAsync(@event.PhotoId);
        if (photo is null) return;

        dbContext.Photos.Remove(photo);

        var installation = await dbContext.Installations.FindAsync(@event.InstallationId);
        if (installation is null) return;

        installation.PhotoCount--;

    }

    private static async Task HandleMeasurementRecorded(ReadModelDbContext dbContext, MeasurementRecorded @event)
    {
        MeasurementReadModel measurement = new()
        {
            Id = @event.MeasurementId,
            InstallationId = @event.InstallationId,
            Type = @event.Type,
            Value = @event.Value,
            Unit = @event.Unit,
            MinThreshold = @event.MinThreshold,
            MaxThreshold = @event.MaxThreshold,
            Result = @event.Result,
            Notes = @event.Notes,
            MeasuredAt = @event.MeasuredAt
        };

        dbContext.Measurements.Add(measurement);

        var installation = await dbContext.Installations.FindAsync(@event.InstallationId);
        if (installation is null) return;

        installation.MeasurementCount++;
    }

    private static async Task HandleMeasurementRemoved(ReadModelDbContext dbContext, MeasurementRemoved @event)
    {
        var measurement = await dbContext.Measurements.FindAsync(@event.MeasurementId);
        if (measurement is null) return;

        dbContext.Measurements.Remove(measurement);

        var installation = await dbContext.Installations.FindAsync(@event.InstallationId);
        if (installation is null) return;

        installation.MeasurementCount--;
    }

    private static async Task HandlePositionUpdated(ReadModelDbContext dbContext, InstallationPositionUpdated @event)
    {
        var installation = await dbContext.Installations.FindAsync(@event.InstallationId);
        if (installation is null) return;

        installation.Latitude = @event.Latitude;
        installation.Longitude = @event.Longitude;
        installation.Altitude = @event.Altitude;
        installation.HorizontalAccuracy = @event.HorizontalAccuracy;
        installation.GpsSource = @event.GpsSource;
        installation.CorrectionService = @event.CorrectionService;
        installation.RtkFixStatus = @event.RtkFixStatus;
        installation.SatelliteCount = @event.SatelliteCount;
        installation.Hdop = @event.Hdop;
        installation.CorrectionAge = @event.CorrectionAge;
        installation.QualityGrade = @event.QualityGrade;
    }

    private static async Task HandleDescriptionUpdated(ReadModelDbContext dbContext, InstallationDescriptionUpdated @event)
    {
        var installation = await dbContext.Installations.FindAsync(@event.InstallationId);
        if (installation is null) return;

        installation.Description = @event.Description;
    }

    private static async Task HandleCableSpecUpdated(ReadModelDbContext dbContext, InstallationCableSpecUpdated @event)
    {
        var installation = await dbContext.Installations.FindAsync(@event.InstallationId);
        if (installation is null) return;

        installation.CableType = @event.CableType;
        installation.CrossSection = @event.CrossSection;
        installation.CableColor = @event.CableColor;
        installation.ConductorCount = @event.ConductorCount;
    }

    private static async Task HandleDepthUpdated(ReadModelDbContext dbContext, InstallationDepthUpdated @event)
    {
        var installation = await dbContext.Installations.FindAsync(@event.InstallationId);
        if (installation is null) return;

        installation.DepthMm = @event.DepthMm;
    }

    private static async Task HandleDeviceInfoUpdated(ReadModelDbContext dbContext, InstallationDeviceInfoUpdated @event)
    {
        var installation = await dbContext.Installations.FindAsync(@event.InstallationId);
        if (installation is null) return;

        installation.Manufacturer = @event.Manufacturer;
        installation.ModelName = @event.ModelName;
        installation.SerialNumber = @event.SerialNumber;
    }

    private static async Task HandleCompleted(ReadModelDbContext dbContext, InstallationCompleted @event)
    {
        var installation = await dbContext.Installations.FindAsync(@event.InstallationId);
        if (installation is null) return;

        installation.Status = "completed";
        installation.CompletedAt = @event.CompletedAt;
    }

    private static async Task HandleDeleted(ReadModelDbContext dbContext, InstallationDeleted @event)
    {
        var installation = await dbContext.Installations.FindAsync(@event.InstallationId);
        if (installation is null) return;

        installation.IsDeleted = true;
    }
}

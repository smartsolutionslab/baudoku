using BauDoku.Documentation.Domain;
using Marten;
using Marten.Events;
using Weasel.Core;

namespace BauDoku.Documentation.Infrastructure.Persistence;

public static class MartenConfiguration
{
    public static void Configure(StoreOptions options, string connectionString)
    {
        options.Connection(connectionString);
        options.Events.DatabaseSchemaName = "documentation_events";
        options.DatabaseSchemaName = "documentation_events";
        options.Events.StreamIdentity = StreamIdentity.AsGuid;
        options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;

        options.Events.AddEventTypes([
            typeof(InstallationDocumented),
            typeof(PhotoAdded),
            typeof(PhotoRemoved),
            typeof(MeasurementRecorded),
            typeof(MeasurementRemoved),
            typeof(InstallationPositionUpdated),
            typeof(InstallationDescriptionUpdated),
            typeof(InstallationCableSpecUpdated),
            typeof(InstallationDepthUpdated),
            typeof(InstallationDeviceInfoUpdated),
            typeof(InstallationCompleted),
            typeof(InstallationDeleted),
            typeof(LowGpsQualityDetected)
        ]);
    }
}

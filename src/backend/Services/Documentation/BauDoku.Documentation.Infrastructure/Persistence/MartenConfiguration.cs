using BauDoku.BuildingBlocks.Serialization;
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
        options.Events.DatabaseSchemaName = DocumentationSchemas.Events;
        options.DatabaseSchemaName = DocumentationSchemas.Events;
        options.Events.StreamIdentity = StreamIdentity.AsGuid;
        options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;

        options.UseSystemTextJsonForSerialization(configure: stj =>
        {
            stj.Converters.Add(new ValueObjectJsonConverterFactory());
        });

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

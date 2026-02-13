using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Builders;

internal sealed class InstallationBuilder
{
    private InstallationIdentifier id = InstallationIdentifier.New();
    private ProjectIdentifier projectId = ProjectIdentifier.New();
    private ZoneIdentifier? zoneId = ZoneIdentifier.New();
    private InstallationType type = InstallationType.CableTray;
    private GpsPosition position = GpsPosition.Create(48.1351, 11.5820, 520.0, 3.5, "internal_gps");
    private Description? description = Description.From("Kabeltrasse im Erdgeschoss");
    private CableSpec? cableSpec = CableSpec.Create("NYM-J 5x2.5", 25);
    private Depth? depth = Depth.From(600);
    private Manufacturer? manufacturer = Manufacturer.From("Hager");
    private ModelName? modelName = ModelName.From("VZ312N");
    private SerialNumber? serialNumber = SerialNumber.From("SN-12345");

    public InstallationBuilder WithId(InstallationIdentifier value) { id = value; return this; }
    public InstallationBuilder WithProjectId(ProjectIdentifier value) { projectId = value; return this; }
    public InstallationBuilder WithZoneId(ZoneIdentifier? value) { zoneId = value; return this; }
    public InstallationBuilder WithType(InstallationType value) { type = value; return this; }
    public InstallationBuilder WithPosition(GpsPosition value) { position = value; return this; }
    public InstallationBuilder WithDescription(Description? value) { description = value; return this; }
    public InstallationBuilder WithCableSpec(CableSpec? value) { cableSpec = value; return this; }
    public InstallationBuilder WithDepth(Depth? value) { depth = value; return this; }
    public InstallationBuilder WithManufacturer(Manufacturer? value) { manufacturer = value; return this; }
    public InstallationBuilder WithModelName(ModelName? value) { modelName = value; return this; }
    public InstallationBuilder WithSerialNumber(SerialNumber? value) { serialNumber = value; return this; }

    public Installation Build() => Installation.Create(
        id, projectId, zoneId, type, position,
        description, cableSpec, depth, manufacturer, modelName, serialNumber);
}

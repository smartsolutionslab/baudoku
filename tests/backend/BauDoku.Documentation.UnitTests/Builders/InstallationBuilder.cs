using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Builders;

internal sealed class InstallationBuilder
{
    private InstallationId _id = InstallationId.New();
    private Guid _projectId = Guid.NewGuid();
    private Guid? _zoneId = Guid.NewGuid();
    private InstallationType _type = InstallationType.CableTray;
    private GpsPosition _position = new(48.1351, 11.5820, 520.0, 3.5, "internal_gps");
    private Description? _description = new("Kabeltrasse im Erdgeschoss");
    private CableSpec? _cableSpec = new("NYM-J 5x2.5", 25);
    private Depth? _depth = new(600);
    private Manufacturer? _manufacturer = new("Hager");
    private ModelName? _modelName = new("VZ312N");
    private SerialNumber? _serialNumber = new("SN-12345");

    public InstallationBuilder WithId(InstallationId id) { _id = id; return this; }
    public InstallationBuilder WithProjectId(Guid projectId) { _projectId = projectId; return this; }
    public InstallationBuilder WithZoneId(Guid? zoneId) { _zoneId = zoneId; return this; }
    public InstallationBuilder WithType(InstallationType type) { _type = type; return this; }
    public InstallationBuilder WithPosition(GpsPosition position) { _position = position; return this; }
    public InstallationBuilder WithDescription(Description? description) { _description = description; return this; }
    public InstallationBuilder WithCableSpec(CableSpec? cableSpec) { _cableSpec = cableSpec; return this; }
    public InstallationBuilder WithDepth(Depth? depth) { _depth = depth; return this; }
    public InstallationBuilder WithManufacturer(Manufacturer? manufacturer) { _manufacturer = manufacturer; return this; }
    public InstallationBuilder WithModelName(ModelName? modelName) { _modelName = modelName; return this; }
    public InstallationBuilder WithSerialNumber(SerialNumber? serialNumber) { _serialNumber = serialNumber; return this; }

    public Installation Build() => Installation.Create(
        _id, _projectId, _zoneId, _type, _position,
        _description, _cableSpec, _depth, _manufacturer, _modelName, _serialNumber);
}

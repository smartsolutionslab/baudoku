using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Rules;

public sealed class MeasurementTypeMustMatchInstallationType : IBusinessRule
{
    private static readonly HashSet<string> ElectricalInstallationTypes =
    [
        "switchgear", "transformer", "junction_box"
    ];

    private static readonly HashSet<string> ElectricalWithGroundingTypes =
    [
        "switchgear", "transformer", "junction_box", "grounding", "lightning_protection"
    ];

    private static readonly HashSet<string> RestrictedToElectricalOnly =
    [
        "rcd_trip_time", "rcd_trip_current"
    ];

    private static readonly HashSet<string> RestrictedToElectricalWithGrounding =
    [
        "loop_impedance"
    ];

    private readonly InstallationType _installationType;
    private readonly MeasurementType _measurementType;

    public MeasurementTypeMustMatchInstallationType(
        InstallationType installationType,
        MeasurementType measurementType)
    {
        _installationType = installationType;
        _measurementType = measurementType;
    }

    public bool IsBroken()
    {
        if (RestrictedToElectricalOnly.Contains(_measurementType.Value))
            return !ElectricalInstallationTypes.Contains(_installationType.Value);

        if (RestrictedToElectricalWithGrounding.Contains(_measurementType.Value))
            return !ElectricalWithGroundingTypes.Contains(_installationType.Value);

        return false;
    }

    public string Message =>
        $"Messungstyp '{_measurementType.Value}' ist fuer Installationstyp '{_installationType.Value}' nicht zulaessig.";
}

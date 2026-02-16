using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Rules;

public sealed class MeasurementTypeMustMatchInstallationType(
    InstallationType installationType,
    MeasurementType measurementType)
    : IBusinessRule
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

    public bool IsBroken()
    {
        if (RestrictedToElectricalOnly.Contains(measurementType.Value))
            return !ElectricalInstallationTypes.Contains(installationType.Value);

        if (RestrictedToElectricalWithGrounding.Contains(measurementType.Value))
            return !ElectricalWithGroundingTypes.Contains(installationType.Value);

        return false;
    }

    public string Message =>
        $"Messungstyp '{measurementType.Value}' ist fuer Installationstyp '{installationType.Value}' nicht zulaessig.";
}

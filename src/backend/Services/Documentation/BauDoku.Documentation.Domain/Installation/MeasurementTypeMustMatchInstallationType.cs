using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed class MeasurementTypeMustMatchInstallationType(InstallationType installationType, MeasurementType measurementType)
    : IBusinessRule
{
    private static readonly HashSet<string> ElectricalInstallationTypes = ["switchgear", "transformer", "junction_box"];
    private static readonly HashSet<string> ElectricalWithGroundingTypes = ["switchgear", "transformer", "junction_box", "grounding", "lightning_protection"];
    private static readonly HashSet<string> RestrictedToElectricalOnly = ["rcd_trip_time", "rcd_trip_current"];
    private static readonly HashSet<string> RestrictedToElectricalWithGrounding = ["loop_impedance"];

    public bool IsBroken()
    {
        if (RestrictedToElectricalOnly.Contains(measurementType.Value)) return !ElectricalInstallationTypes.Contains(installationType.Value);

        if (RestrictedToElectricalWithGrounding.Contains(measurementType.Value)) return !ElectricalWithGroundingTypes.Contains(installationType.Value);

        return false;
    }

    public string Message => $"Messungstyp '{measurementType.Value}' ist für Installationstyp '{installationType.Value}' nicht zulässig.";
}

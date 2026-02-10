using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record InstallationType : ValueObject
{
    private static readonly HashSet<string> ValidValues =
    [
        "cable_tray", "junction_box", "cable_pull", "conduit",
        "grounding", "lightning_protection", "switchgear", "transformer", "other"
    ];

    public static readonly InstallationType CableTray = new("cable_tray");
    public static readonly InstallationType JunctionBox = new("junction_box");
    public static readonly InstallationType CablePull = new("cable_pull");
    public static readonly InstallationType Conduit = new("conduit");
    public static readonly InstallationType Grounding = new("grounding");
    public static readonly InstallationType LightningProtection = new("lightning_protection");
    public static readonly InstallationType Switchgear = new("switchgear");
    public static readonly InstallationType Transformer = new("transformer");
    public static readonly InstallationType Other = new("other");

    public string Value { get; }

    public InstallationType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Installationstyp darf nicht leer sein.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Ungueltiger Installationstyp: {value}.", nameof(value));
        Value = value;
    }
}

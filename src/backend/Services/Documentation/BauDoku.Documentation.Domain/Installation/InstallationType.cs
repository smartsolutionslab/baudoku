using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationType : IValueObject
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

    private InstallationType(string value) => Value = value;

    public static InstallationType From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Installationstyp darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungültiger Installationstyp: {value}.");
        return new InstallationType(value);
    }

    public static InstallationType? FromNullable(string? value) => value is not null ? From(value) : null;
}

namespace BauDoku.Projects.ReadModel;

public sealed record ZoneDto(Guid Id, string Name, string Type, Guid? ParentZoneId);

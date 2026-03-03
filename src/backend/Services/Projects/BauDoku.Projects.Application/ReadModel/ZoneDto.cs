namespace BauDoku.Projects.Application.ReadModel;

public sealed record ZoneDto(Guid Id, string Name, string Type, Guid? ParentZoneId);

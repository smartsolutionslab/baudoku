namespace BauDoku.Projects.Application.Queries.Dtos;

public sealed record ZoneDto(Guid Id, string Name, string Type, Guid? ParentZoneId);

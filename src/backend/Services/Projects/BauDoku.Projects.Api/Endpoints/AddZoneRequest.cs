namespace BauDoku.Projects.Api.Endpoints;

public sealed record AddZoneRequest(string Name, string Type, Guid? ParentZoneId);

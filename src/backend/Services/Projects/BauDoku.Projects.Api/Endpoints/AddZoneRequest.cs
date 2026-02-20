using BauDoku.Projects.Domain;

namespace BauDoku.Projects.Api.Endpoints;

public sealed record AddZoneRequest(ZoneName Name, ZoneType Type, ZoneIdentifier? ParentZoneId);

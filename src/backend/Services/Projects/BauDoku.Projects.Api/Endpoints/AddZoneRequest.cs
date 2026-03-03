using SmartSolutionsLab.BauDoku.Projects.Domain;

namespace SmartSolutionsLab.BauDoku.Projects.Api.Endpoints;

public sealed record AddZoneRequest(ZoneName Name, ZoneType Type, ZoneIdentifier? ParentZoneId);

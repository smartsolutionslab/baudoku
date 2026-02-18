using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.GetInstallation;

public sealed record GetInstallationQuery(Guid InstallationId) : IQuery<InstallationDto>;

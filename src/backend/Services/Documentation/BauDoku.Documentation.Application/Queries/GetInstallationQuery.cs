using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Queries;

public sealed record GetInstallationQuery(InstallationIdentifier InstallationId) : IQuery<InstallationDto>;

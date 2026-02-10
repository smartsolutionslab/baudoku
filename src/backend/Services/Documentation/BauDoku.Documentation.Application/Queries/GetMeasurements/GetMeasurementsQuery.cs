using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.Queries.Dtos;

namespace BauDoku.Documentation.Application.Queries.GetMeasurements;

public sealed record GetMeasurementsQuery(Guid InstallationId) : IQuery<IReadOnlyList<MeasurementDto>>;

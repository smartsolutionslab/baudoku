using FluentValidation;

namespace BauDoku.Documentation.Application.Commands.RemoveMeasurement;

public sealed class RemoveMeasurementCommandValidator : AbstractValidator<RemoveMeasurementCommand>
{
    public RemoveMeasurementCommandValidator()
    {
        RuleFor(x => x.InstallationId).NotEmpty();
        RuleFor(x => x.MeasurementId).NotEmpty();
    }
}

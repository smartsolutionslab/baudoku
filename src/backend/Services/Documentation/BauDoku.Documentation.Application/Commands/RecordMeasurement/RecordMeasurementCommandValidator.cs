using BauDoku.Documentation.Domain.ValueObjects;
using FluentValidation;

namespace BauDoku.Documentation.Application.Commands.RecordMeasurement;

public sealed class RecordMeasurementCommandValidator : AbstractValidator<RecordMeasurementCommand>
{
    public RecordMeasurementCommandValidator()
    {
        RuleFor(x => x.InstallationId).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.Value).GreaterThan(0);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(MeasurementUnit.MaxLength);
    }
}

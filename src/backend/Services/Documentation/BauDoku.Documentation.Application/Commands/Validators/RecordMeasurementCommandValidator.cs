using FluentValidation;

namespace SmartSolutionsLab.BauDoku.Documentation.Application.Commands.Validators;

public sealed class RecordMeasurementCommandValidator : AbstractValidator<RecordMeasurementCommand>
{
    public RecordMeasurementCommandValidator()
    {
        RuleFor(x => x.InstallationId).NotNull();
        RuleFor(x => x.Type).NotNull();
        RuleFor(x => x.Unit).NotNull();
        RuleFor(x => x.Value).GreaterThanOrEqualTo(0)
            .WithMessage("Messwert darf nicht negativ sein.");
        RuleFor(x => x)
            .Must(x => !x.MinThreshold.HasValue || !x.MaxThreshold.HasValue || x.MinThreshold.Value <= x.MaxThreshold.Value)
            .WithMessage("MinThreshold darf nicht groesser als MaxThreshold sein.");
    }
}

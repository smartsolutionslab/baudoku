using BauDoku.Documentation.Domain;
using FluentValidation;

namespace BauDoku.Documentation.Application.Commands.DocumentInstallation;

public sealed class DocumentInstallationCommandValidator : AbstractValidator<DocumentInstallationCommand>
{
    public DocumentInstallationCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.HorizontalAccuracy).GreaterThan(0);
        RuleFor(x => x.GpsSource).NotEmpty();

        RuleFor(x => x.Description).MaximumLength(Description.MaxLength)
            .When(x => x.Description is not null);
        RuleFor(x => x.CableType).MaximumLength(CableType.MaxLength)
            .When(x => x.CableType is not null);
        RuleFor(x => x.Manufacturer).MaximumLength(Manufacturer.MaxLength)
            .When(x => x.Manufacturer is not null);
        RuleFor(x => x.ModelName).MaximumLength(ModelName.MaxLength)
            .When(x => x.ModelName is not null);
        RuleFor(x => x.SerialNumber).MaximumLength(SerialNumber.MaxLength)
            .When(x => x.SerialNumber is not null);
        RuleFor(x => x.CrossSection).GreaterThan(0)
            .When(x => x.CrossSection is not null);
        RuleFor(x => x.DepthMm).GreaterThanOrEqualTo(0)
            .When(x => x.DepthMm is not null);
    }
}

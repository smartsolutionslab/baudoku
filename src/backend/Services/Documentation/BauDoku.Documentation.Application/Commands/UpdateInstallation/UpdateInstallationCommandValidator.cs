using BauDoku.Documentation.Domain.ValueObjects;
using FluentValidation;

namespace BauDoku.Documentation.Application.Commands.UpdateInstallation;

public sealed class UpdateInstallationCommandValidator : AbstractValidator<UpdateInstallationCommand>
{
    public UpdateInstallationCommandValidator()
    {
        RuleFor(x => x.InstallationId).NotEmpty();

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.Longitude.HasValue);

        RuleFor(x => x.HorizontalAccuracy)
            .GreaterThan(0)
            .When(x => x.HorizontalAccuracy.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(Description.MaxLength)
            .When(x => x.Description is not null);

        RuleFor(x => x.CableType)
            .MaximumLength(CableType.MaxLength)
            .When(x => x.CableType is not null);

        RuleFor(x => x.CrossSection)
            .GreaterThan(0)
            .When(x => x.CrossSection.HasValue);

        RuleFor(x => x.DepthMm)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DepthMm.HasValue);
    }
}

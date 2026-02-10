using BauDoku.Projects.Domain.ValueObjects;
using FluentValidation;

namespace BauDoku.Projects.Application.Commands.AddZone;

public sealed class AddZoneCommandValidator : AbstractValidator<AddZoneCommand>
{
    public AddZoneCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(ZoneName.MaxLength);
        RuleFor(x => x.Type).NotEmpty();
    }
}

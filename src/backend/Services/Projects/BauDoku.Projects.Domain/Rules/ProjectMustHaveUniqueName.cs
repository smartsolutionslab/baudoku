using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain.Rules;

public sealed class ProjectMustHaveUniqueName : IBusinessRule
{
    private readonly bool _nameAlreadyExists;

    public ProjectMustHaveUniqueName(bool nameAlreadyExists)
    {
        _nameAlreadyExists = nameAlreadyExists;
    }

    public bool IsBroken() => _nameAlreadyExists;

    public string Message => "Ein Projekt mit diesem Namen existiert bereits.";
}

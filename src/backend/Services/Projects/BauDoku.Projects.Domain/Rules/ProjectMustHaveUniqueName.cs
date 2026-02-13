using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain.Rules;

public sealed class ProjectMustHaveUniqueName : IBusinessRule
{
    private readonly bool nameAlreadyExists;

    public ProjectMustHaveUniqueName(bool nameAlreadyExists)
    {
        this.nameAlreadyExists = nameAlreadyExists;
    }

    public bool IsBroken() => nameAlreadyExists;

    public string Message => "Ein Projekt mit diesem Namen existiert bereits.";
}

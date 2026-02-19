using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain;

public sealed class ProjectMustHaveUniqueName(bool nameAlreadyExists) : IBusinessRule
{
    public bool IsBroken() => nameAlreadyExists;

    public string Message => "Ein Projekt mit diesem Namen existiert bereits.";
}

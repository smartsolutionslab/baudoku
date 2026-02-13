namespace BauDoku.BuildingBlocks.Domain.Guards;

public ref struct GuidGuard
{
    private readonly Guid value;
    private readonly string paramName;

    internal GuidGuard(Guid value, string paramName)
    {
        this.value = value;
        this.paramName = paramName;
    }

    public GuidGuard IsNotEmpty(string? message = null)
    {
        if (value == Guid.Empty)
            throw new ArgumentException(
                message ?? $"{paramName} darf nicht leer sein.", paramName);
        return this;
    }
}

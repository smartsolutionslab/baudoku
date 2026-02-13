namespace BauDoku.BuildingBlocks.Domain.Guards;

public ref struct ReferenceGuard<T> where T : class
{
    private readonly T? value;
    private readonly string paramName;

    internal ReferenceGuard(T? value, string paramName)
    {
        this.value = value;
        this.paramName = paramName;
    }

    public ReferenceGuard<T> IsNotNull(string? message = null)
    {
        if (value is null)
            throw new ArgumentNullException(paramName,
                message ?? $"{paramName} darf nicht null sein.");
        return this;
    }
}

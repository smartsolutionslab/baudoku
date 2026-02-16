using System.Numerics;

namespace BauDoku.BuildingBlocks.Domain.Guards;

public ref struct NumericGuard<T> where T : struct, INumber<T>
{
    private readonly T value;
    private readonly string paramName;

    internal NumericGuard(T value, string paramName)
    {
        this.value = value;
        this.paramName = paramName;
    }

    public NumericGuard<T> IsGreaterThan(T min, string? message = null)
    {
        if (value <= min)
            throw new ArgumentOutOfRangeException(paramName, message ?? $"{paramName} muss groesser als {min} sein.");
        return this;
    }

    public NumericGuard<T> IsNotNegative(string? message = null)
    {
        if (value < T.Zero)
            throw new ArgumentOutOfRangeException(paramName, message ?? $"{paramName} darf nicht negativ sein.");
        return this;
    }

    public NumericGuard<T> IsPositive(string? message = null)
    {
        if (value <= T.Zero)
            throw new ArgumentOutOfRangeException(paramName, message ?? $"{paramName} muss groesser als 0 sein.");
        return this;
    }

    public NumericGuard<T> IsBetween(T min, T max, string? message = null)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(paramName, message ?? $"{paramName} muss zwischen {min} und {max} liegen.");
        return this;
    }
}

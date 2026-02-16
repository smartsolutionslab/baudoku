using System.Numerics;

namespace BauDoku.BuildingBlocks.Domain.Guards;

public ref struct NullableNumericGuard<T> where T : struct, INumber<T>
{
    private readonly T? value;
    private readonly string paramName;

    internal NullableNumericGuard(T? value, string paramName)
    {
        this.value = value;
        this.paramName = paramName;
    }

    public NullableNumericGuard<T> IsGreaterThan(T min, string? message = null)
    {
        if (value.HasValue && value.Value <= min)
            throw new ArgumentOutOfRangeException(paramName, message ?? $"{paramName} muss groesser als {min} sein.");
        return this;
    }

    public NullableNumericGuard<T> IsNotNegative(string? message = null)
    {
        if (value.HasValue && value.Value < T.Zero)
            throw new ArgumentOutOfRangeException(paramName, message ?? $"{paramName} darf nicht negativ sein.");
        return this;
    }

    public NullableNumericGuard<T> IsPositive(string? message = null)
    {
        if (value.HasValue && value.Value <= T.Zero)
            throw new ArgumentOutOfRangeException(paramName, message ?? $"{paramName} muss groesser als 0 sein.");
        return this;
    }

    public NullableNumericGuard<T> IsBetween(T min, T max, string? message = null)
    {
        if (value.HasValue && (value.Value < min || value.Value > max))
            throw new ArgumentOutOfRangeException(paramName, message ?? $"{paramName} muss zwischen {min} und {max} liegen.");
        return this;
    }
}

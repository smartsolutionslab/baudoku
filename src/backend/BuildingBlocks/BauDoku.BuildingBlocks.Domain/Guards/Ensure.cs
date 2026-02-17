using System.Runtime.CompilerServices;

namespace BauDoku.BuildingBlocks.Domain.Guards;

public static class Ensure
{
    public static StringGuard That(string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => new(value, paramName ?? "value");

    public static GuidGuard That(Guid value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => new(value, paramName ?? "value");

    public static NumericGuard<int> That(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => new(value, paramName ?? "value");

    public static NumericGuard<long> That(long value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => new(value, paramName ?? "value");

    public static NumericGuard<double> That(double value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => new(value, paramName ?? "value");

    public static NullableNumericGuard<int> That(int? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => new(value, paramName ?? "value");

    public static NullableNumericGuard<long> That(long? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => new(value, paramName ?? "value");

    public static NullableNumericGuard<double> That(double? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => new(value, paramName ?? "value");

    public static NumericGuard<decimal> That(decimal value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => new(value, paramName ?? "value");

    public static NullableNumericGuard<decimal> That(decimal? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => new(value, paramName ?? "value");

    public static ReferenceGuard<T> That<T>(T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : class
        => new(value, paramName ?? "value");
}

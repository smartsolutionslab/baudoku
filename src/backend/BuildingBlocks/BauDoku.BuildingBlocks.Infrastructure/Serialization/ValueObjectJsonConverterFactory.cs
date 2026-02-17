using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.BuildingBlocks.Infrastructure.Serialization;

public sealed class ValueObjectJsonConverterFactory : JsonConverterFactory
{
    private static readonly ConcurrentDictionary<Type, JsonConverter?> ConverterCache = new();

    public override bool CanConvert(Type typeToConvert)
        => typeof(IValueObject).IsAssignableFrom(typeToConvert)
           && !typeToConvert.IsAbstract
           && !typeToConvert.IsInterface;

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converter = ConverterCache.GetOrAdd(typeToConvert, BuildConverter);
        return converter ?? throw new JsonException($"Cannot create converter for ValueObject type {typeToConvert.Name}.");
    }

    private static JsonConverter? BuildConverter(Type voType)
    {
        var valueProperty = voType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
        if (valueProperty is null) return null;

        var fromMethod = voType.GetMethod("From", BindingFlags.Public | BindingFlags.Static, [valueProperty.PropertyType]);

        var converterType = typeof(ValueObjectJsonConverter<,>).MakeGenericType(voType, valueProperty.PropertyType);
        return (JsonConverter)Activator.CreateInstance(converterType, fromMethod)!;
    }
}

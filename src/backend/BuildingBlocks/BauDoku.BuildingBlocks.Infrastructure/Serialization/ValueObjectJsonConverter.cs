using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BauDoku.BuildingBlocks.Infrastructure.Serialization;

public sealed class ValueObjectJsonConverter<TVo, TValue>(MethodInfo? fromMethod) : JsonConverter<TVo>
{
    public override TVo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = JsonSerializer.Deserialize<TValue>(ref reader, options);
        if (value is null) return default;

        if (fromMethod is not null) return (TVo)fromMethod.Invoke(null, [value])!;

        // Fallback: try constructor with single value parameter
        var constructor = typeToConvert.GetConstructor([typeof(TValue)]);
        if (constructor is not null) return (TVo)constructor.Invoke([value]);

        throw new JsonException($"Cannot deserialize ValueObject {typeToConvert.Name}: no From() method or matching constructor found.");
    }

    public override void Write(Utf8JsonWriter writer, TVo value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        var valueProperty = typeof(TVo).GetProperty("Value")!;
        var innerValue = valueProperty.GetValue(value);

        JsonSerializer.Serialize(writer, innerValue, typeof(TValue), options);
    }
}

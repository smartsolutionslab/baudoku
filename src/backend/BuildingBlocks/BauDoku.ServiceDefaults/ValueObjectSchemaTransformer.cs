using System.Reflection;
using BauDoku.BuildingBlocks.Domain;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BauDoku.ServiceDefaults;

public sealed class ValueObjectSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var type = context.JsonTypeInfo.Type;

        if (!typeof(IValueObject).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
            return Task.CompletedTask;

        var valueProperty = type.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
        if (valueProperty is null) return Task.CompletedTask;

        var (schemaType, format) = MapToOpenApiType(valueProperty.PropertyType);

        schema.Type = schemaType;
        schema.Format = format;
        schema.Properties?.Clear();
        schema.Required?.Clear();
        schema.AdditionalProperties = null;

        var maxLengthField = type.GetField("MaxLength", BindingFlags.Public | BindingFlags.Static);
        if (maxLengthField is not null && maxLengthField.GetValue(null) is int maxLength)
        {
            schema.MaxLength = maxLength;
        }

        return Task.CompletedTask;
    }

    private static (JsonSchemaType Type, string? Format) MapToOpenApiType(Type clrType) => clrType switch
    {
        _ when clrType == typeof(Guid) => (JsonSchemaType.String, "uuid"),
        _ when clrType == typeof(string) => (JsonSchemaType.String, null),
        _ when clrType == typeof(int) => (JsonSchemaType.Integer, "int32"),
        _ when clrType == typeof(long) => (JsonSchemaType.Integer, "int64"),
        _ when clrType == typeof(double) => (JsonSchemaType.Number, "double"),
        _ when clrType == typeof(float) => (JsonSchemaType.Number, "float"),
        _ when clrType == typeof(decimal) => (JsonSchemaType.Number, "decimal"),
        _ when clrType == typeof(bool) => (JsonSchemaType.Boolean, null),
        _ when clrType == typeof(DateTime) => (JsonSchemaType.String, "date-time"),
        _ when clrType == typeof(DateTimeOffset) => (JsonSchemaType.String, "date-time"),
        _ => (JsonSchemaType.String, null)
    };
}

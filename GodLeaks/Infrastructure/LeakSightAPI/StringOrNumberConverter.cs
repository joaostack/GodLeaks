using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GodLeaks.Infrastructure.LeakSightAPI;

/// <summary>
/// Accepts API fields that are inconsistently returned as JSON strings or numbers.
/// </summary>
public sealed class StringOrNumberConverter : JsonConverter<string>
{
    public override string? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number => reader.GetDecimal()
                .ToString(CultureInfo.InvariantCulture),
            JsonTokenType.Null => null,
            _ => throw new JsonException(
                $"Expected a string, number, or null, but found {reader.TokenType}.")
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        string? value,
        JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value);
    }
}

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GodLeaks.Infrastructure.LeakSightAPI;

/// <summary>
/// Accepts API fields that are inconsistently returned as JSON strings or numbers.
/// </summary>
public sealed class StringOrNumberConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        var type = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
        return type == typeof(string) || type == typeof(int) || type == typeof(long) ||
               type == typeof(short) || type == typeof(byte) || type == typeof(uint) ||
               type == typeof(ulong) || type == typeof(ushort) || type == typeof(sbyte) ||
               type == typeof(float) || type == typeof(double) || type == typeof(decimal);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(StringOrNumberValueConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private sealed class StringOrNumberValueConverter<T> : JsonConverter<T>
    {
        private static readonly Type TargetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return default;

            var text = reader.TokenType == JsonTokenType.String
                ? reader.GetString()
                : reader.GetDecimal().ToString(CultureInfo.InvariantCulture);

            if (TargetType == typeof(string))
                return (T?)(object?)text;

            return (T?)Convert.ChangeType(text, TargetType, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            if (TargetType == typeof(string))
                writer.WriteStringValue((string)(object)value);
            else if (value is int i) writer.WriteNumberValue(i);
            else if (value is long l) writer.WriteNumberValue(l);
            else if (value is short s) writer.WriteNumberValue(s);
            else if (value is byte b) writer.WriteNumberValue(b);
            else if (value is uint ui) writer.WriteNumberValue(ui);
            else if (value is ulong ul) writer.WriteNumberValue(ul);
            else if (value is ushort us) writer.WriteNumberValue(us);
            else if (value is sbyte sb) writer.WriteNumberValue(sb);
            else if (value is float f) writer.WriteNumberValue(f);
            else if (value is double d) writer.WriteNumberValue(d);
            else if (value is decimal m) writer.WriteNumberValue(m);
        }
    }
}

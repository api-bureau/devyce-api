using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiBureau.Devyce.Api.Converters;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _format;

    public CustomDateTimeConverter(string format) => _format = format;

    public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
    {
        writer.WriteStringValue(date.ToString(_format));
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var dateString = reader.GetString();

            if (string.IsNullOrWhiteSpace(dateString)) return default;

            if (DateTime.TryParseExact(dateString, _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                return parsedDate;
            }

            // Fallback to general parse (culture-invariant)
            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind | DateTimeStyles.AllowWhiteSpaces, out parsedDate))
            {
                return parsedDate;
            }

            throw new JsonException($"Unable to parse DateTime value: '{dateString}'.");
        }

        throw new JsonException($"Unexpected token parsing DateTime. TokenType: {reader.TokenType}");
    }
}

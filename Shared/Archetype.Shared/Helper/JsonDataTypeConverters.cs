namespace Archetype.Shared.Helper;

public class NumberToStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Handle reading the value based on the actual type
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt64().ToString();
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            string? str = reader.GetString();
            return str ?? string.Empty;
        }

        throw new JsonException("Unexpected token type");
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        // Handle writing the value
        writer.WriteStringValue(value);
    }
}

public class StringToInt32Converter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Handle reading the value based on the actual type
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32();
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return Convert.ToInt32(reader.GetString());
        }
        throw new JsonException("Unexpected token type");
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        // Handle writing the value
        writer.WriteStringValue(value.ToString());
    }
}

public class StringToInt64Converter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Handle reading the value based on the actual type
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt64();
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return Convert.ToInt64(reader.GetString());
        }

        throw new JsonException("Unexpected token type");
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        // Handle writing the value
        writer.WriteStringValue(value.ToString());
    }
}

public class StringToDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (DateTime.TryParse(reader.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }
            else
            {
                throw new JsonException("Invalid DateTime format");
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64()).UtcDateTime;
        }

        throw new JsonException("Unexpected token type for DateTime");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss")); // Adjust the format as needed
    }
}

public class StringToTimeSpanConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (TimeSpan.TryParse(reader.GetString(), CultureInfo.InvariantCulture, out TimeSpan timeSpan))
            {
                return timeSpan;
            }
            else
            {
                throw new JsonException("Invalid TimeSpan format");
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return TimeSpan.FromMilliseconds(reader.GetInt64());
        }

        throw new JsonException("Unexpected token type for TimeSpan");
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString()); // Adjust the format as needed
    }
}

public class UnixTimestampToDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64()).DateTime;
        }

        throw new JsonException("Unexpected token type for TimeSpan");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString()); // Adjust the format as needed
    }
}

public class SecondsToDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return DateTime.UtcNow.AddSeconds(reader.GetInt64());
        }
        if (reader.TokenType == JsonTokenType.String)
        {
            if (DateTime.TryParse(reader.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }
            else
            {
                throw new JsonException("Invalid DateTime format");
            }
        }

        throw new JsonException("Unexpected token type for TimeSpan");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString()); // Adjust the format as needed
    }
}

public sealed class FlexibleNullableDateConverter : JsonConverter<DateTime?>
{
    // Accept a few common variants; order matters (most strict first).
    private static readonly string[] _formats =
    [
        "yyyy-MM-dd",    // canonical (ISO-like without time)
        "dd-MM-yyyy",
        "d-M-yyyy",
        "MM-dd-yyyy",
        "M-d-yyyy",
        "yyyy/MM/dd",
        "dd/MM/yyyy",
        "d/M/yyyy"
    ];

    public override DateTime? Read(ref Utf8JsonReader reader,
                                   Type typeToConvert,
                                   JsonSerializerOptions options)
    {
        // null -> null
        if (reader.TokenType == JsonTokenType.Null) return null;

        // ISO 8601 fast-path (handles "yyyy-MM-dd" directly and cheaply)
        if (reader.TokenType == JsonTokenType.String && reader.TryGetDateTime(out DateTime iso))
            return iso;

        if (reader.TokenType == JsonTokenType.String)
        {
            string? s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s)) return null;
            s = s.Trim();

            // Try exact formats first (no exceptions)
            foreach (string fmt in _formats)
            {
                if (DateTime.TryParseExact(s, fmt, CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out DateTime dt))
                    return dt;
            }

            // Heuristic for "7-9-1980" and similar with separators (- / . space)
            string[] parts = s.Split('-', '/', '.', ' ');
            if (parts.Length == 3 &&
                parts[2].Length == 4 &&
                int.TryParse(parts[0], out int a) &&
                int.TryParse(parts[1], out int b) &&
                int.TryParse(parts[2], out int y))
            {
                // Try interpret as d-M-yyyy
                if (a >= 1 && a <= 31 && b >= 1 && b <= 12)
                {
                    if (DateTime.TryParseExact($"{a:D2}-{b:D2}-{y}", "dd-MM-yyyy",
                                               CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dmy))
                        return dmy;

                    // Or M-d-yyyy (US style)
                    if (DateTime.TryParseExact($"{a:D2}-{b:D2}-{y}", "MM-dd-yyyy",
                                               CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime mdy))
                        return mdy;
                }
            }

            // Last resort: permissive parse (still no exceptions thrown)
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture,
                                  DateTimeStyles.AllowWhiteSpaces, out DateTime any))
                return any;
        }

        // Non-string tokens aren’t supported for this field
        throw new JsonException("Invalid date token.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null) { writer.WriteNullValue(); return; }
        // Normalize output
        writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}

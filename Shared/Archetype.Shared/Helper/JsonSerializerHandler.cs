namespace Archetype.Shared.Helper;

public static class JsonSerializerHandler
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    // -------------------------
    // TValue receiver extensions
    // -------------------------
    extension<TValue>(TValue value)
    {
        public string? Serialize(JsonSerializerOptions? options = null)
        {
            if (value is null) return null;
            return JsonSerializer.Serialize(value, options ?? _options);
        }
    }

    // -------------------------
    // string receiver extensions
    // -------------------------
    extension(string json)
    {
        public TValue? Deserialize<TValue>(JsonSerializerOptions? options = null)
        {
            if (string.IsNullOrWhiteSpace(json)) return default;

            // Preserve your "string passthrough" behavior:
            if (typeof(TValue) == typeof(string))
                return (TValue)(object)json;

            return JsonSerializer.Deserialize<TValue>(json, options ?? _options);
        }

        /// <summary>
        /// Parses the JSON, rebuilds it while excluding the specified properties,
        /// then deserializes the clean result. No regex. No string mutation.
        /// </summary>
        public TValue? DeserializeExcluding<TValue>(JsonSerializerOptions? options = null,
                                                    params ReadOnlySpan<string> excludedProperties)
        {
            if (string.IsNullOrWhiteSpace(json)) return default;

            if (typeof(TValue) == typeof(string))
                return (TValue)(object)json;

            if (excludedProperties.IsEmpty)
                return JsonSerializer.Deserialize<TValue>(json, options ?? _options);

            using JsonDocument document = JsonDocument.Parse(json);
            using MemoryStream stream = new();
            using Utf8JsonWriter writer = new(stream);

            writer.WriteStartObject();

            foreach (JsonProperty property in document.RootElement.EnumerateObject())
            {
                bool shouldSkip = false;
                foreach (string excluded in excludedProperties)
                {
                    if (property.Name.Equals(excluded, StringComparison.OrdinalIgnoreCase))
                    {
                        shouldSkip = true;
                        break;
                    }
                }

                if (!shouldSkip)
                    property.WriteTo(writer);
            }

            writer.WriteEndObject();
            writer.Flush();

            stream.Position = 0;
            return JsonSerializer.Deserialize<TValue>(stream, options ?? _options);
        }
    }
}
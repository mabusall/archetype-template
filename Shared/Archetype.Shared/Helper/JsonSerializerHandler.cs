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
        /// Deserialize JSON with sanitization that removes the malformed property.
        /// </summary>
        public TValue? DeserializeWithSanitization<TValue>(string propName, JsonSerializerOptions? options = null)
        {
            if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(propName))
                return default;

            if (typeof(TValue) == typeof(string))
                return (TValue)(object)json;

            // Remove: "propName": "....",  (string value), optionally trailing comma
            string pattern = $@"""{Regex.Escape(propName)}""\s*:\s*""(?:[^""\\]|\\.)*""\s*,?";

            string sanitized = Regex.Replace(json, pattern, "", RegexOptions.Singleline);

            sanitized = sanitized.Replace(",,", ",")
                                 .Replace(",}", "}")
                                 .Replace("{,", "{")
                                 .Replace(",]", "]");

            return JsonSerializer.Deserialize<TValue>(sanitized, options ?? _options);
        }
    }
}
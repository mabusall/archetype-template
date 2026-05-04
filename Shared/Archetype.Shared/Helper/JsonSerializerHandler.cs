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

            if (typeof(TValue).Equals(typeof(string)))
                return (TValue)Convert.ChangeType(json, typeof(TValue));

            // Pre-processing: Fix malformed escape sequences in OMDB responses

            // Fix 1: Replace invalid \, with \\, (comma is not a valid escape character)
            json = json.Replace(@"\,", @"\\,");

            // Fix 2: Fix properties ending with backslash followed by quote-comma
            // Pattern: text\ "," -> text\" ","
            json = Regex.Replace(json, @"(\\)("",)", "\\\"$2", RegexOptions.Singleline);

            // Use RegEx to find and remove the problematic property
            // Pattern explanation:
            // - "Plot"\s*:\s* - Match property name with colon and whitespace
            // - "(?:[^"\\]|\\.)* - Match string value (handles escaped quotes)
            // - "\s*,? - Match closing quote and optional comma
            // Use RegexOptions.Singleline to match across line breaks
            string pattern = $@",?\s*""{Regex.Escape(propName)}""\s*:\s*""(?:[^""\\]|\\.)*""\s*,?";

            // Remove the property (and surrounding commas if present)
            json = Regex.Replace(json, pattern, ",", RegexOptions.Singleline);

            // If the property still exists (meaning it was too malformed for regex), use aggressive removal
            string propCheck = $@"""{Regex.Escape(propName)}""\s*:";
            if (Regex.IsMatch(json, propCheck))
            {
                Match propMatch = Regex.Match(json, propCheck);
                if (propMatch.Success)
                {
                    int propStart = propMatch.Index;

                    // Find the next valid property (pattern: ,"PropertyName":)
                    int searchStart = propMatch.Index + propMatch.Length;
                    string nextPropPattern = @",\s*""[A-Za-z]+""\s*:";
                    Regex nextPropRegex = new(nextPropPattern);
                    Match nextPropMatch = nextPropRegex.Match(json, searchStart);

                    if (nextPropMatch.Success)
                    {
                        // Remove from the start of the problematic property to just before the comma of the next property
                        int removeLength = nextPropMatch.Index - propStart;
                        json = json.Remove(propStart, removeLength);
                    }
                }
            }

            // Clean up potential formatting issues left behind
            json = json.Replace(",,", ",")
                       .Replace(",}", "}")   // Remove trailing comma before closing brace
                       .Replace("{,", "{")   // Remove leading comma after opening brace
                       .Replace(",]", "]");  // Remove trailing comma before closing bracket

            return JsonSerializer.Deserialize<TValue>(json, options ?? _options);
        }
    }
}
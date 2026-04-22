namespace Archetype.Shared.Extensions;

public static class ObjectExtention
{
    extension(string s)
    {
        /// <summary>
        /// Converts a consumer type string to an endpoint name with prefix.
        /// Example: "Archetype.Notification.EventHandlers.Handlers.SendEmailConsumer, FlixHub.Notification"
        /// with prefix "archetype-" becomes "tasheer-send-email-consumer"
        /// </summary>
        public string ToEndpointName(string prefix = "")
        {
            if (string.IsNullOrWhiteSpace(s))
                return string.Empty;

            // Split by comma to separate type from assembly
            string[] parts = s.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length == 0)
                return string.Empty;

            // Get the full type name (before the comma)
            string fullTypeName = parts[0];

            // Extract just the class name (after the last dot)
            int lastDotIndex = fullTypeName.LastIndexOf('.');
            string className = lastDotIndex >= 0
                ? fullTypeName[(lastDotIndex + 1)..]
                : fullTypeName;

            // Convert to kebab-case
            string kebabName = className.ToKebabCase();

            // Add prefix
            return string.IsNullOrWhiteSpace(prefix)
                ? kebabName
                : $"{prefix}{kebabName}";
        }

        public string ToKebabCase()
        {
            if (string.IsNullOrWhiteSpace(s))
                return string.Empty;

            StringBuilder sb = new(s.Length + 8);

            // Tracks previous character categories to decide when to insert '-'
            bool prevWasSeparator = true; // treat start like separator
            bool prevWasLower = false;
            bool prevWasLetter = false;
            bool prevWasDigit = false;

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                // Treat any whitespace or common separators as word breaks
                if (char.IsWhiteSpace(c) || c is '-' or '_' or '.' or '/' or '\\')
                {
                    prevWasSeparator = true;
                    prevWasLower = false;
                    prevWasLetter = false;
                    prevWasDigit = false;
                    continue;
                }

                bool isLetter = char.IsLetter(c);
                bool isDigit = char.IsDigit(c);

                // Insert '-' on word boundaries:
                // 1) lower -> upper (helloWorld)
                // 2) acronym boundary: "HTTPServer" -> "http-server"
                //    Detect: prev upper + current upper + next lower
                // 3) letter <-> digit transitions: "Version2" -> "version-2"
                bool needDash = false;

                if (!prevWasSeparator && sb.Length > 0)
                {
                    if (isLetter)
                    {
                        bool isUpper = char.IsUpper(c);

                        if (isUpper && prevWasLower)
                        {
                            // helloWorld -> hello-world
                            needDash = true;
                        }
                        else if (isUpper && prevWasLetter && !prevWasLower && i + 1 < s.Length && char.IsLower(s[i + 1]))
                            needDash = true;
                    }
                    else if (isDigit)
                    {
                        if (prevWasLetter && !prevWasSeparator)
                            needDash = true; // Version2 -> version-2
                    }
                    else
                    {
                        // For other unicode categories (symbols, punctuation),
                        // treat as separator (skip)
                        prevWasSeparator = true;
                        prevWasLower = false;
                        prevWasLetter = false;
                        prevWasDigit = false;
                        continue;
                    }

                    // digit -> letter boundary: 2FA -> 2-fa
                    if (isLetter && prevWasDigit)
                        needDash = true;
                }

                if (needDash && sb.Length > 0 && sb[^1] != '-')
                    sb.Append('-');

                // Append normalized lowercase letter/digit
                if (isLetter)
                {
                    sb.Append(char.ToLowerInvariant(c));
                    prevWasLower = char.IsLower(c);
                    prevWasLetter = true;
                    prevWasDigit = false;
                }
                else if (isDigit)
                {
                    sb.Append(c);
                    prevWasLower = false;
                    prevWasLetter = false;
                    prevWasDigit = true;
                }

                prevWasSeparator = false;
            }

            // Trim trailing '-' if any
            while (sb.Length > 0 && sb[^1] == '-')
                sb.Length--;

            return sb.ToString();
        }
    }
}
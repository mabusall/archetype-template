namespace Archetype.Infrastructure.Authentications;

public static class HttpClientAuthenticationExtension
{
    extension(HttpClient client)
    {
        public HttpClient AttachBasicAuthentication(string userName, string password)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(password);
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Username cannot be null or empty.", nameof(userName));

            // Basic auth: "username:password" base64
            byte[] credentials = Encoding.UTF8.GetBytes($"{userName}:{password}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));

            return client;
        }

        public HttpClient AttachBearerAuthentication(string token)
        {
            ArgumentNullException.ThrowIfNull(client);
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        /// <summary>
        /// Attaches an Authorization header with a custom scheme/value (e.g. "ApiKey", "token").
        /// Note: Many APIs expect API keys in a custom header (e.g. "x-api-key") instead of Authorization.
        /// </summary>
        public HttpClient AttachApiKeyAuthentication(string scheme, string value)
        {
            ArgumentNullException.ThrowIfNull(client);
            if (string.IsNullOrWhiteSpace(scheme))
                throw new ArgumentException("Scheme cannot be null or empty.", nameof(scheme));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(scheme, value);

            return client;
        }

        /// <summary>
        /// Helper for APIs that require API keys in a custom header (common pattern).
        /// Example: client.AttachHeader("x-api-key", apiKey);
        /// </summary>
        public HttpClient AttachHeader(string headerName, string headerValue)
        {
            ArgumentNullException.ThrowIfNull(client);
            if (string.IsNullOrWhiteSpace(headerName))
                throw new ArgumentException("Header name cannot be null or empty.", nameof(headerName));
            ArgumentNullException.ThrowIfNull(headerValue);

            client.DefaultRequestHeaders.Remove(headerName);
            client.DefaultRequestHeaders.Add(headerName, headerValue);

            return client;
        }
    }
}

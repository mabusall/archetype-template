namespace Archetype.Infrastructure.Services;

public class ApiKeyFilter : IEndpointFilter
{
    // You can choose a different header name.
    private readonly string _apiKeyHeaderName = "X-Api-Key";

    public static string ApiKey { get; set; } = default!;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
                                                EndpointFilterDelegate next)
    {
        HttpContext httpContext = context.HttpContext;

        if (httpContext.RequestAborted.IsCancellationRequested)
        {
            return Results.StatusCode(StatusCodes.Status499ClientClosedRequest);
        }

        // Read the API key from configuration
        if (!httpContext.Request.Headers.TryGetValue(_apiKeyHeaderName, out StringValues apiKey) ||
            !apiKey.Equals(ApiKey))
            throw new InvalidApiKeyException("Invalid API Key");

        // Proceed to the next filter or handler if ApiKey are satisfied
        return await next(context);
    }
}
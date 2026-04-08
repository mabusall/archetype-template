namespace Archetype.Infrastructure.Services;

public class CorrelationIdService(IHttpContextAccessor httpContextAccessor)
    : ICorrelationIdService
{
    public string Get()
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return string.Empty;
        }

        // Preferred source: items (set by CorrelationIdMiddleware)
        if (httpContext.Items.TryGetValue(IdGeneratorService.CorrelationIdKey, out object? value) &&
            value is string s &&
            !string.IsNullOrWhiteSpace(s))
        {
            return s;
        }

        // Fallback to request header (e.g. in non-HTTP scenarios this will be empty)
        if (httpContext.Request.Headers.TryGetValue(IdGeneratorService.CorrelationIdKey, out StringValues headerValue) &&
            !string.IsNullOrWhiteSpace(headerValue))
        {
            return headerValue.ToString();
        }

        return string.Empty;
    }
}

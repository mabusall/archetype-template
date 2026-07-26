namespace Archetype.Shared.Exceptions.Handler;

public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
                                                Exception exception,
                                                CancellationToken cancellationToken)
    {

        // If the client aborted the request, do not write a response.
        if (exception is OperationCanceledException ||
            exception is TaskCanceledException ||
            httpContext.RequestAborted.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning("Request was canceled/aborted by the client [Path: {Path}, TraceId: {TraceId}]",
                              httpContext.Request.Path,
                              httpContext.TraceIdentifier);

                // Optionally: set a non-standard 499 for server logs (client won't receive it)
                httpContext.Response.StatusCode = 499; // "Client Closed Request" as used by Nginx
            }

            return true; // swallow; nothing to write back
        }

        // Map exception type to HTTP status code
        int statusCode = exception switch
        {
            InternalServerException => StatusCodes.Status500InternalServerError,
            FluentValidation.ValidationException => StatusCodes.Status400BadRequest,
            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            InvalidApiKeyException => StatusCodes.Status401Unauthorized,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            PublishOutboxException => StatusCodes.Status400BadRequest,
            RateLimitExceededException => StatusCodes.Status429TooManyRequests,
            _ => StatusCodes.Status500InternalServerError,
        };

        // Get correlationId from middleware (if available)
        string? correlationId =
            httpContext.Items.TryGetValue(IdGeneratorService.CorrelationIdKey, out object? cid)
                ? cid?.ToString()
                : string.Empty;

        string traceId = httpContext.TraceIdentifier;
        string path = httpContext.Request.Path.Value ?? string.Empty;

        // OpenTelemetry / Activity tagging
        Activity? activity = Activity.Current;
        if (activity is not null)
        {
            if (!string.IsNullOrWhiteSpace(correlationId))
                activity.SetTag(IdGeneratorService.CorrelationIdKey, correlationId);
            activity.SetTag("exception.type", exception.GetType().Name);
            activity.SetTag("exception.message", exception.Message);
            activity.SetTag("exception.stacktrace", exception.StackTrace);
            activity.SetTag("http.path", path);
            activity.SetStatus(ActivityStatusCode.Error);
        }

        // Structured logging
        logger.LogError(exception,
                        "Unhandled exception {ExceptionType} at {Path} with status {StatusCode} [TraceId: {TraceId}]",
                        exception.GetType().Name,
                        path,
                        statusCode,
                        traceId);

        // Build ProblemDetails
        ProblemDetails problemDetails = new()
        {
            Title = exception.GetType().Name,
            Detail = exception.Message,
            Status = statusCode,
            Instance = path
        };

        problemDetails.Extensions["traceId"] = traceId;

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            problemDetails.Extensions[IdGeneratorService.CorrelationIdKey] = correlationId;
        }

        if (exception is FluentValidation.ValidationException validationException)
        {
            problemDetails.Extensions["validationErrors"] = validationException.Errors;
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = MediaTypeNames.Application.ProblemJson;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

        return true;
    }
}

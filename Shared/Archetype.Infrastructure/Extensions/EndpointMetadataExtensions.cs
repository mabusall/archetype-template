namespace Archetype.Infrastructure.Extensions;

public static class EndpointMetadataExtensions
{
    extension(RouteHandlerBuilder endpoint)
    {
        public RouteHandlerBuilder WithStandardEndpointMetadata<TResult>(string name,
                                                                         string summary,
                                                                         string? description = null,
                                                                         int successStatusCode = StatusCodes.Status200OK,
                                                                         string? contentType = null,
                                                                         bool includeUnauthorized = true,
                                                                         bool includeForbidden = true)
        {
            RouteHandlerBuilder result = endpoint
                .WithName(name)
                .WithSummary(summary)
                .WithDescription(description ?? summary)
                .Produces<TResult>(successStatusCode, contentType)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            if (includeUnauthorized)
            {
                result = result.ProducesProblem(StatusCodes.Status401Unauthorized);
            }
            if (includeForbidden)
            {
                result = result.ProducesProblem(StatusCodes.Status403Forbidden);
            }

            return result;
        }
    }
}
namespace Archetype.Infrastructure.Services;

public class PermissionFilter(int[] requiredPermissions) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
                                                EndpointFilterDelegate next)
    {
        if (requiredPermissions is not null && requiredPermissions.Length != 0)
        {
            HttpContext httpContext = context.HttpContext;

            // Retrieve user claims or permissions (adjust based on your authentication setup)
            List<int> userPermissions = httpContext.User.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => int.Parse(c.Value))
                .ToList();

            // Check if the user has all required permissions
            if (!requiredPermissions.All(permissions => userPermissions.Contains(permissions)))
            {
                return Results.Forbid();
            }
        }

        // Proceed to the next filter or handler if permissions are satisfied
        return await next(context);
    }
}

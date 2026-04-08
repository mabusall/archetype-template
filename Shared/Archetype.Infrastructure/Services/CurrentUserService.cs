namespace Archetype.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public ClaimsPrincipal User => httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    public string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    public string UserName => User.Identity?.Name ?? string.Empty;
}

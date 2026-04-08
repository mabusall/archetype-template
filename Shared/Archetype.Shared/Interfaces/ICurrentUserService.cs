namespace Archetype.Shared.Interfaces;

public interface ICurrentUserService
{
    ClaimsPrincipal User { get; }
    public string UserId { get; }
    public string UserName { get; }
}

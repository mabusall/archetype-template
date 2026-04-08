namespace Archetype.Keycloak.Api.Features.Realm;

public class KeycloakRealmRefreshTokenCommandValidator : AbstractValidator<KeycloakRealmRefreshTokenCommand>
{
    public KeycloakRealmRefreshTokenCommandValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}
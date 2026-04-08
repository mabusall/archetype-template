namespace Archetype.Keycloak.Api.Features.Realm;

public class KeycloakRealmLogoutCommandValidator : AbstractValidator<KeycloakRealmLogoutCommand>
{
    public KeycloakRealmLogoutCommandValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}
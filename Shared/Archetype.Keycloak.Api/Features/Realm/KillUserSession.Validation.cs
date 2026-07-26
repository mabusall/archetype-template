namespace Archetype.Keycloak.Api.Features.Realm;

public class KeycloakRealmKillUserSessionCommandValidator : AbstractValidator<KeycloakRealmKillUserSessionCommand>
{
    public KeycloakRealmKillUserSessionCommandValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}
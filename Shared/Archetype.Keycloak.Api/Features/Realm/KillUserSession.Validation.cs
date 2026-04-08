namespace Archetype.Keycloak.Api.Features.Realm;

public class KeycloakRealmKillUserSessionCommandValidator : AbstractValidator<KeycloakRealmKillUserSessionCommand>
{
    public KeycloakRealmKillUserSessionCommandValidator()
    {
        RuleFor(r => r.SessionId)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}
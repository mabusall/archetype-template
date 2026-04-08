namespace Archetype.Keycloak.Api.Features.Realm;

public class KeycloakRealmExtendUserSessionCommandValidator : AbstractValidator<KeycloakRealmExtendUserSessionCommand>
{
    public KeycloakRealmExtendUserSessionCommandValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}
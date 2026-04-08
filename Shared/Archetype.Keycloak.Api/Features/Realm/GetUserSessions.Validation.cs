namespace Archetype.Keycloak.Api.Features.Realm;

public class KeycloakRealmGetUserSessionsQueryValidator : AbstractValidator<KeycloakRealmGetUserSessionsQuery>
{
    public KeycloakRealmGetUserSessionsQueryValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}
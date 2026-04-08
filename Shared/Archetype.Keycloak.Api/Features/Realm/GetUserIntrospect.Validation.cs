namespace Archetype.Keycloak.Api.Features.Realm;

public class KeycloakRealmGetUserIntrospectQueryValidator : AbstractValidator<KeycloakRealmGetUserIntrospectQuery>
{
    public KeycloakRealmGetUserIntrospectQueryValidator()
    {
        RuleFor(r => r.AccessToken)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}
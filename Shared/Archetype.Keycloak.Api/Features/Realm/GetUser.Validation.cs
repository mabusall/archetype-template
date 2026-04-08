namespace Archetype.Keycloak.Api.Features.Realm;

public class KeycloakRealmGetUserQueryValidator : AbstractValidator<KeycloakRealmGetUserQuery>
{
    public KeycloakRealmGetUserQueryValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}
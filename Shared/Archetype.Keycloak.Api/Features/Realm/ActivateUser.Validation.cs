namespace Archetype.Keycloak.Api.Features.Realm;

public class KeycloakRealmUpdateUserCommandValidator : AbstractValidator<KeycloakRealmActivateUserCommand>
{
    public KeycloakRealmUpdateUserCommandValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}

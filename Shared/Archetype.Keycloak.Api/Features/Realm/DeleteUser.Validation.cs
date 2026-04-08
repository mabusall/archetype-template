namespace Archetype.Keycloak.Api.Features.Realm;

public class KeycloakRealmDeleteUserCommandValidator : AbstractValidator<KeycloakRealmDeleteUserCommand>
{
    public KeycloakRealmDeleteUserCommandValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}
namespace Archetype.Keycloak.Api.Features.Realm;

public class KeycloakRealmLoginCommandValidator : AbstractValidator<KeycloakRealmLoginCommand>
{
    public KeycloakRealmLoginCommandValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);

        RuleFor(r => r.Password)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}
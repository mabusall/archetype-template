namespace Archetype.Keycloak.Api.Features.Realm;

public class KeycloakRealmResetPasswordCommandValidator : AbstractValidator<KeycloakRealmResetPasswordCommand>
{
    public KeycloakRealmResetPasswordCommandValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);

        RuleFor(r => r.Password)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}
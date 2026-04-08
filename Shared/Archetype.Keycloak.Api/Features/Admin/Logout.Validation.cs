namespace Archetype.Keycloak.Api.Features.Admin;

public class KeycloakAdminLogoutCommandValidator : AbstractValidator<KeycloakAdminLogoutCommand>
{
    public KeycloakAdminLogoutCommandValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}
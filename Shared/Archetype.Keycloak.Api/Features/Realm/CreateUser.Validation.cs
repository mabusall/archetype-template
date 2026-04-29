namespace Archetype.Keycloak.Api.Features.Realm;

public class KeycloakRealmCreateUserCommandValidator : AbstractValidator<KeycloakRealmCreateUserCommand>
{
    public KeycloakRealmCreateUserCommandValidator()
    {
        RuleFor(r => r.UserName)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
        RuleFor(r => r.Email)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
        RuleFor(r => r.FirstName)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
        RuleFor(r => r.LastName)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
        RuleFor(r => r.Password)
            .NotEmpty()
            .WithMessage(ErrorMessageResources.NotEmpty);
    }
}
namespace Archetype.Keycloak.Api.Features.Admin;

public record KeycloakAdminLogoutCommand(string RefreshToken) : ICommand<KeycloakSuccessStatusResult>;
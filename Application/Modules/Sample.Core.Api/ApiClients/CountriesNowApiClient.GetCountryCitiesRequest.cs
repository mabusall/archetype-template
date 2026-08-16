namespace Sample.Core.Api.ApiClients;

internal record GetCountryCitiesRequest
{
    public string Country { get; init; } = string.Empty;
}

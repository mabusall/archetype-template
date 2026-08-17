namespace Sample.Core.Api.ApiClients;

internal interface ICountriesNowApiClient
{
    Task<GetCountryCitiesResponse> GetCitiesAsync(string countryName, CancellationToken cancellationToken = default);
}

internal class CountriesNowApiClient(HttpClient httpClient,
                                     AppSettingsKeyManagement appSettings)
    : ICountriesNowApiClient
{
    public IntegrationApi CountriesNowConf { get; } = appSettings.IntegrationApisOptions.Apis["CountriesNow"];

    public async Task<GetCountryCitiesResponse> GetCitiesAsync(string countryName, CancellationToken cancellationToken = default)
    {
        using FormUrlEncodedContent content = new(new Dictionary<string, string>
        {
            ["country"] = countryName,
        });
        using HttpRequestMessage httpRequest = new(HttpMethod.Post, "cities")
        {
            Content = content
        };

        HttpResponseMessage response = await httpClient.SendAsync(httpRequest, cancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<GetCountryCitiesResponse>(cancellationToken))!;
    }
}
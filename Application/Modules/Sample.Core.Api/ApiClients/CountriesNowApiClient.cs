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
        GetCountryCitiesRequest request = new() { Country = countryName };

        using StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.FormUrlEncoded);

        HttpResponseMessage response = await httpClient.PostAsync("cities", content, cancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<GetCountryCitiesResponse>(cancellationToken))!;
    }
}
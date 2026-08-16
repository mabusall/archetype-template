namespace Sample.Core.Api.Model;

public record GetCountryCitiesResponse
{
    public bool Error { get; init; }

    public string Msg { get; init; } = string.Empty;

    public IList<string> Data { get; init; } = [];
}

namespace DigitalGarden.Shared.Models.Options;

public class TraktOptions
{
    public const string SectionName = "Trakt";

    public string BaseAddress { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public int RefreshTokensWithXDaysLeft { get; set; } = 2;
    public TraktOptionsEndpoints Endpoints { get; set; } = new();
}

public class TraktOptionsEndpoints
{
    public string GetNewTokens { get; set; } = string.Empty;
    public string GetWatchedMovies { get; set; } = string.Empty;
    public string GetWatchedShows {  get; set; } = string.Empty;
}

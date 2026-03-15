namespace DigitalGarden.Shared.Models.Options;

public class LastFmOptions
{
    public const string SectionName = "LastFm";

    public string BaseAddress { get; set; } = string.Empty;
    public string TopArtistsEndpoint { get; set; } = string.Empty;
    public string TopTracksEndpoint { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
}

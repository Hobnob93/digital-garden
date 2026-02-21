using System.Text.Json.Serialization;

namespace DigitalGarden.Shared.Models.Data;

public sealed record LastFmTopArtistsResponse
(
    [property: JsonPropertyName("topartists")] LastFmTopArtists TopArtists
);

public sealed record LastFmTopArtists
(
    [property: JsonPropertyName("artist")] LastFmArtist[] Artists
);

public sealed record LastFmArtist
(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("playcount")] int PlayCount
);

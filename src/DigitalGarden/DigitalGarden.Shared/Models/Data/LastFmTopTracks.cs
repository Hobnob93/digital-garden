using System.Text.Json.Serialization;

namespace DigitalGarden.Shared.Models.Data;

public sealed record LastFmTopTracksResponse
(
    [property: JsonPropertyName("toptracks")] LastFmTopTracks TopTracks
);

public sealed record LastFmTopTracks
(
    [property: JsonPropertyName("track")] LastFmTrack[] Tracks
);

public sealed record LastFmTrack
(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("artist")] LastFmTrackArtist Artist,
    [property: JsonPropertyName("playcount")] int PlayCount
);

public sealed record LastFmTrackArtist
(
    [property: JsonPropertyName("name")] string Name
);

using System.Text.Json.Serialization;

namespace DigitalGarden.Shared.Models.Data.Responses;

public record TraktWatchedShowRoot
(
    [property: JsonPropertyName("plays")] int Plays,
    [property: JsonPropertyName("last_watched_at")] DateTime LastWatchedAtUtc,
    [property: JsonPropertyName("last_updated_at")] DateTime LastUpdatedAtUtc,
    [property: JsonPropertyName("show")] TraktWatchedShow Show,
    [property: JsonPropertyName("seasons")] TraktWatchedShowSeason[] Seasons
);

public record TraktWatchedShow
(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("year")] int ReleaseYear,
    [property: JsonPropertyName("ids")] TraktWatchedShowIds Ids
);

public record TraktWatchedShowIds
(
    [property: JsonPropertyName("trakt")] int Trakt,
    [property: JsonPropertyName("slug")] string Slug,
    [property: JsonPropertyName("tvdb")] int Tvdb,
    [property: JsonPropertyName("imdb")] string Imdb,
    [property: JsonPropertyName("tmdb")] int Tmdb
);

public record TraktWatchedShowSeason
(
    [property: JsonPropertyName("number")] int SeasonNumber,
    [property: JsonPropertyName("episodes")] TraktWatchedShowEpisode[] Episodes
);

public record TraktWatchedShowEpisode
(
    [property: JsonPropertyName("number")] int EpisodeNumber,
    [property: JsonPropertyName("plays")] int Plays,
    [property: JsonPropertyName("last_watched_at")] DateTime LastWatchedUtc
);

using System.Text.Json.Serialization;

namespace DigitalGarden.Shared.Models.Data.Responses;

public record TraktWatchedMovieRoot
(
    [property: JsonPropertyName("plays")] int Plays,
    [property: JsonPropertyName("last_watched_at")] DateTime LastWatchedAtUtc,
    [property: JsonPropertyName("last_updated_at")] DateTime LastUpdatedAtUtc,
    [property: JsonPropertyName("movie")] TraktWatchedMovie Movie
);

public record TraktWatchedMovie
(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("year")] int ReleaseYear,
    [property: JsonPropertyName("ids")] TraktWatchedMovieIds Ids
);

public record TraktWatchedMovieIds
(
    [property: JsonPropertyName("trakt")] int Trakt,
    [property: JsonPropertyName("slug")] string Slug,
    [property: JsonPropertyName("imdb")] string Imdb,
    [property: JsonPropertyName("tmdb")] int Tmdb
);

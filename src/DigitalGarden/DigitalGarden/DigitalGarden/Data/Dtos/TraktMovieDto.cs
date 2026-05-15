namespace DigitalGarden.Data.Dtos;

public class TraktMovieDto
{
    public int TraktId { get; set; }
    public string ImdbId { get; set; } = string.Empty;
    public int TmdbId { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public DateTime LastWatchedUtc { get; set; }
    public int PlayCount { get; set; }
}

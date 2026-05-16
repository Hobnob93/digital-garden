namespace DigitalGarden.Data.Dtos;

public class LastFmSnapshotDto
{
    public int Id { get; set; }
    public DateTime CapturedAtUtc { get; set; }
    public string Period { get; set; } = "6month";

    public List<TopArtistEntryDto> TopArtists { get; set; } = [];
    public List<TopTrackEntryDto> TopTracks { get; set; } = [];
}

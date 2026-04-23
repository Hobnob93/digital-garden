namespace DigitalGarden.Data.Dtos;

public class TopTrackEntryDto
{
    public int Id { get; set; }
    public int Rank { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;
    public int PlayCount { get; set; }

    public int SnapshotId { get; set; }
    public LastFmSnapshotDto? Snapshot { get; set; }
}

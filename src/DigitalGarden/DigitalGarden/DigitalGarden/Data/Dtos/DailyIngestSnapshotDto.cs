namespace DigitalGarden.Data.Dtos;

public class DailyIngestSnapshotDto
{
    public int Id { get; set; }
    public DateTime CapturedAtUtc { get; set; }
    public DateTime? FullGameFetchAtUtc { get; set; }
}

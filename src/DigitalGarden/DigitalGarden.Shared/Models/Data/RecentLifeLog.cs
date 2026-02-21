using DigitalGarden.Shared.Models.Enums;

namespace DigitalGarden.Shared.Models.Data;

public class RecentLifeLog
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public LifeLogItemType Type { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime AddedAtUtc { get; set; }
}

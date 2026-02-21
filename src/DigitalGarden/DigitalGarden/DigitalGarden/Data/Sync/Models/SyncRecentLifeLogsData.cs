using DigitalGarden.Data.Sync.Models.Base;

namespace DigitalGarden.Data.Sync.Models;

public class SyncRecentLifeLogsData : BaseSyncSlugData
{
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsCurrent { get; set; }
}

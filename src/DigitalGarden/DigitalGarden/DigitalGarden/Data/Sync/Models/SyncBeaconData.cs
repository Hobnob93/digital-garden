using DigitalGarden.Data.Sync.Models.Base;

namespace DigitalGarden.Data.Sync.Models;

public class SyncBeaconData : BaseSyncSlugData
{
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

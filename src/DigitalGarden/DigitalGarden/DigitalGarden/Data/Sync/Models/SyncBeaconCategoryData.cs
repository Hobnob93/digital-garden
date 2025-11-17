using DigitalGarden.Data.Sync.Models.Base;

namespace DigitalGarden.Data.Sync.Models;

public class SyncBeaconCategoryData : BaseSyncSlugData
{
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

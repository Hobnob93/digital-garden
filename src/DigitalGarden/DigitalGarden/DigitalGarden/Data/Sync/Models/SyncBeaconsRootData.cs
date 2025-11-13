namespace DigitalGarden.Data.Sync.Models;

public class SyncBeaconsRootData
{
    public SyncBeaconCategoryData[] Categories { get; set; } = [];
    public Dictionary<string, SyncBeaconData[]> BeaconsInCategories { get; set; } = [];
}

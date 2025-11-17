namespace DigitalGarden.Shared.Models.Data;

public class BeaconCategoryItems : BeaconCategory
{
    public ICollection<Beacon> Beacons { get; set; } = [];
}

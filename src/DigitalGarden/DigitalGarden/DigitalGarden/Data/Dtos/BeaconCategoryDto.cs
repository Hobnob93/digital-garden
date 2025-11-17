namespace DigitalGarden.Data.Dtos;

public class BeaconCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public ICollection<BeaconDto> Beacons { get; set; } = [];
}

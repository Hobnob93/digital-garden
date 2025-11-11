namespace DigitalGarden.Data.Dtos;

public class BeaconDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public BeaconCategoryDto? Category { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime AddedAtUtc { get; set; }
}

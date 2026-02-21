using DigitalGarden.Shared.Models.Enums;

namespace DigitalGarden.Data.Dtos;

public class LifeLogItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public LifeLogItemType Type { get; set; }
    public bool IsCurrent { get; set; }

    public string Slug { get; set; } = string.Empty;
    public DateTime AddedAtUtc { get; set; }
}

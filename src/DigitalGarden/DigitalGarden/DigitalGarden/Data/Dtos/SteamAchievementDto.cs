namespace DigitalGarden.Data.Dtos;

public class SteamAchievementDto
{
    public int Id { get; set; }
    public int AppId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UnlockedIcon { get; set; } = string.Empty;
    public string LockedIcon { get; set; } = string.Empty;

    public double? GlobalUnlockPercent { get; set; }
    public DateTime? GlobalPercentUpdatedUtc { get; set; }

    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedAtUtc { get; set; }

    public SteamGameDto? Game { get; set; }
}

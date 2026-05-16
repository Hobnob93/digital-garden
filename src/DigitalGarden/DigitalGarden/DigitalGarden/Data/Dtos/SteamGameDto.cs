namespace DigitalGarden.Data.Dtos;

public class SteamGameDto
{
    public int AppId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime LastPlayedUtc { get; set; }
    public int TotalPlayTimeMinutes { get; set; }
    public bool IsCompleted { get; set; }
    public bool HaveAllAchievements { get; set; }
    public DateTime? LastFullUpdateUtc { get; set; }

    public List<SteamAchievementDto> Achievements { get; set; } = [];
}

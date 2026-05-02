using System.Text.Json.Serialization;

namespace DigitalGarden.Shared.Models.Data;

public sealed record SteamSchemaForGameResponse
(
    [property: JsonPropertyName("game")] SteamSchemaForGameRoot Root
)
{
    public bool GameHasAchievements => Root.Stats.Achievements.Length > 0;
}

public sealed record SteamSchemaForGameRoot
(
    [property: JsonPropertyName("gameVersion")] string GameVersion,
    [property: JsonPropertyName("availableGameStats")] SteamSchemaForGameStats Stats
);

public sealed record SteamSchemaForGameStats
(
    [property: JsonPropertyName("achievements")] SteamSchemaForGameAchievement[] Achievements
);

public sealed record SteamSchemaForGameAchievement
(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("displayName")] string DisplayName,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("icon")] string UnlockedIconUrl,
    [property: JsonPropertyName("icongray")] string LockedIconUrl
)
{
    public string UnlockedIcon => Path.GetFileName(new Uri(UnlockedIconUrl).AbsolutePath);
    public string LockedIcon => Path.GetFileName(new Uri(LockedIconUrl).AbsolutePath);
};

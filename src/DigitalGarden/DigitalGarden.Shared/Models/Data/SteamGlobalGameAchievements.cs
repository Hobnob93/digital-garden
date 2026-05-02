using System.Text.Json.Serialization;

namespace DigitalGarden.Shared.Models.Data;

public sealed record SteamGlobalGameAchievementsResponse
(
    [property: JsonPropertyName("achievementpercentages")] SteamGlobalGameAchievementsRoot Root
);

public sealed record SteamGlobalGameAchievementsRoot
(
    [property: JsonPropertyName("achievements")] SteamGlobalGameAchievement[] Achievements
);

public sealed record SteamGlobalGameAchievement
(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("percent")] double PercentageUnlocked
);

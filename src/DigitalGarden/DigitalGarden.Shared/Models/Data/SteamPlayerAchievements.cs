using System.Text.Json.Serialization;

namespace DigitalGarden.Shared.Models.Data;

public sealed record SteamPlayerAchievementsResponse
(
    [property: JsonPropertyName("playerstats")] SteamPlayerAchievementsRoot Root
);

public sealed record SteamPlayerAchievementsRoot
(
    [property: JsonPropertyName("error")] string? Error,
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("achievements")] SteamPlayerAchievement[] Achievements
);

public sealed record SteamPlayerAchievement
(
    [property: JsonPropertyName("apiname")] string Name,
    [property: JsonPropertyName("achieved")] int Unlocked,
    [property: JsonPropertyName("unlocktime")] int UnlockedAtUnixSeconds
);

using System.Text.Json.Serialization;

namespace DigitalGarden.Shared.Models.Data;

public sealed record SteamOwnedGamesResponse
(
    [property: JsonPropertyName("response")] SteamOwnedGamesRoot Root
);

public sealed record SteamOwnedGamesRoot
(
    [property: JsonPropertyName("game_count")] int GameCount,
    [property: JsonPropertyName("games")] SteamOwnedGamesGame[] Games
);

public sealed record SteamOwnedGamesGame
(
    [property: JsonPropertyName("appid")] int AppId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("playtime_forever")] int TotalPlayTime,
    [property: JsonPropertyName("rtime_last_played")] int LastPlayedUnixSeconds
);

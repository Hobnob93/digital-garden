namespace DigitalGarden.Shared.Models.Options;

public class SteamOptions
{
    public const string SectionName = "Steam";

    public string BaseAddress { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public int MaxFullUpdates { get; set; } = 100;
    public int UpdateDelayDays { get; set; } = 14;
    public int FullFetchDelayDays { get; set; } = 3;
    public SteamOptionsEndpoints Endpoints { get; set; } = new();
}

public class SteamOptionsEndpoints
{
    public string GetOwnedGames { get; set; } = string.Empty;
    public string GetPlayerAchievements { get; set; } = string.Empty;
    public string GetSchemaForGame { get; set; } = string.Empty;
    public string GetGlobalGameAchievements { get; set; } = string.Empty;
}

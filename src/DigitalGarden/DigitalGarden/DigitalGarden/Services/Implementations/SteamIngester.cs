using DigitalGarden.Data;
using DigitalGarden.Data.Dtos;
using DigitalGarden.Extensions;
using DigitalGarden.Services.Interfaces;
using DigitalGarden.Shared.Constants;
using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Models.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DigitalGarden.Services.Implementations;

public class SteamIngester : IGameIngester
{
    private const int PoliteDelayMs = 200;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SteamOptions _steamOptions;
    private readonly ILogger<SteamIngester> _logger;

    public SteamIngester(IHttpClientFactory httpClientFactory, IOptions<SteamOptions> steamOptions, ILogger<SteamIngester> logger)
    {
        _httpClientFactory = httpClientFactory;
        _steamOptions = steamOptions.Value;
        _logger = logger;
    }

    public async Task<bool> ShouldDoFullFetch(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var lastRunUtc = await dbContext.DailySnapshots
            .Where(s => s.FullGameFetchAtUtc.HasValue)
            .OrderByDescending(s => s.CapturedAtUtc)
            .Select(s => s.FullGameFetchAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (!lastRunUtc.HasValue)
            return true;

        return lastRunUtc.Value.AddDays(_steamOptions.FullFetchDelayDays) <= DateTime.UtcNow;
    }

    public async Task RunIngestAsync(ApplicationDbContext dbContext, DateTime ingestTimeUtc, bool shouldDoFullFetch, CancellationToken cancellationToken)
    {
        if (shouldDoFullFetch)
        {
            var allGamesResponse = await GetAllOwnedGamesAsync(cancellationToken);
            var playedGamesResponse = allGamesResponse.Root.Games.Where(g => g.TotalPlayTime > 0);
            foreach (var gameData in playedGamesResponse)
            {
                try
                {
                    await HandleGameResponseAsync(dbContext, gameData, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errored adding game '{GameName}' to the database", gameData.Name);
                }
            }
        }

        var gamesToUpdate = await dbContext.SteamGames
            .Where(g => !g.LastFullUpdateUtc.HasValue)
            .Take(_steamOptions.MaxFullUpdates)
            .ToListAsync(cancellationToken);

        var remainingUpdateCount = _steamOptions.MaxFullUpdates - gamesToUpdate.Count;
        if (remainingUpdateCount > 0)
        {
            var extraGamesToUpdate = await dbContext.SteamGames
                .Where(g => g.LastFullUpdateUtc.HasValue && g.LastFullUpdateUtc.Value.AddDays(_steamOptions.UpdateDelayDays) <= DateTime.UtcNow)
                .Take(remainingUpdateCount)
                .ToArrayAsync(cancellationToken);
            gamesToUpdate.AddRange(extraGamesToUpdate);
        }

        foreach (var gameToUpdate in gamesToUpdate)
        {
            try
            {
                var gameSchema = await GetGameSchemaAsync(gameToUpdate.AppId, cancellationToken);
                var myAchievements = await GetGamePlayerAchievementsAsync(gameSchema.GameHasAchievements, gameToUpdate.AppId, cancellationToken);
                var globalAchievements = await GetGameGlobalAchievementsAsync(gameSchema.GameHasAchievements, gameToUpdate.AppId, cancellationToken);

                await HandleAchievementsUpdateAsync(dbContext, gameToUpdate, gameSchema, globalAchievements, myAchievements, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errored fully updating game '{GameName}'", gameToUpdate.Name);
            }

            // Respect Steam's servers with a short delay between each game...
            await Task.Delay(PoliteDelayMs, cancellationToken);
        }
    }

    private async Task HandleGameResponseAsync(ApplicationDbContext dbContext, SteamOwnedGamesGame steamGameData, CancellationToken cancellationToken)
    {
        var game = await dbContext.SteamGames.FindAsync([steamGameData.AppId], cancellationToken);
        var isNewGame = false;
        if (game is null)
        {
            isNewGame = true;
            game = new SteamGameDto
            {
                AppId = steamGameData.AppId
            };
        }

        game.Name = steamGameData.Name;
        game.TotalPlayTimeMinutes = steamGameData.TotalPlayTime;
        game.LastPlayedUtc = DateTimeOffset.FromUnixTimeSeconds(steamGameData.LastPlayedUnixSeconds).UtcDateTime;

        if (isNewGame)
        {
            dbContext.SteamGames.Add(game);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleAchievementsUpdateAsync(ApplicationDbContext dbContext, SteamGameDto game, SteamSchemaForGameResponse gameSchema, SteamGlobalGameAchievementsResponse globalAchievements, SteamPlayerAchievementsResponse myAchievements, CancellationToken cancellationToken)
    {
        game.LastFullUpdateUtc = DateTime.UtcNow;

        // Exit if app has no achievements
        if (!gameSchema.GameHasAchievements)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var achievementNamesCache = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var existingAchievements = (await dbContext.SteamAchievements
            .Where(a => a.AppId == game.AppId)
            .ToArrayAsync(cancellationToken))
            .ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);

        // Add the achievements list from the schema
        foreach (var achievementSchema in gameSchema.Root.Stats.Achievements)
        {
            achievementNamesCache.Add(achievementSchema.Name);

            if (existingAchievements.TryGetValue(achievementSchema.Name, out var existingAchievementData))
            {
                existingAchievementData.DisplayName = achievementSchema.DisplayName;
                existingAchievementData.Description = achievementSchema.Description ?? string.Empty;
                existingAchievementData.LockedIcon = achievementSchema.LockedIcon;
                existingAchievementData.UnlockedIcon = achievementSchema.UnlockedIcon;
            }
            else
            {
                var newAchievementData = new SteamAchievementDto
                {
                    Name = achievementSchema.Name,
                    AppId = game.AppId,
                    UnlockedIcon = achievementSchema.UnlockedIcon,
                    LockedIcon = achievementSchema.LockedIcon,
                    DisplayName = achievementSchema.DisplayName,
                    Description = achievementSchema.Description ?? string.Empty
                };

                await dbContext.SteamAchievements.AddAsync(newAchievementData, cancellationToken);
                existingAchievements.Add(newAchievementData.Name, newAchievementData);
            }
        }

        // Update achievements data for global values
        foreach (var globalAchievementData in globalAchievements.Root.Achievements)
        {
            var achievementData = existingAchievements[globalAchievementData.Name];
            achievementData.GlobalUnlockPercent = globalAchievementData.PercentageUnlocked;
            achievementData.GlobalPercentUpdatedUtc = DateTime.UtcNow;
        }

        // Update achievement data with my own achievement collection
        foreach (var ownAchievementData in myAchievements.Root.Achievements)
        {
            var achievementData = existingAchievements[ownAchievementData.Name];
            achievementData.IsUnlocked = ownAchievementData.Unlocked != 0;
            if (achievementData.IsUnlocked)
                achievementData.UnlockedAtUtc = DateTimeOffset.FromUnixTimeSeconds(ownAchievementData.UnlockedAtUnixSeconds).UtcDateTime;
        }

        // Delete any achievements no longer part of the game
        var deletionTargets = await dbContext.SteamAchievements
            .Where(dto => dto.AppId == game.AppId && !achievementNamesCache.Contains(dto.Name))
            .ToArrayAsync(cancellationToken);

        if (deletionTargets.Length > 0)
            dbContext.SteamAchievements.RemoveRange(deletionTargets);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<SteamOwnedGamesResponse> GetAllOwnedGamesAsync(CancellationToken cancellationToken)
    {
        var endpoint = string.Format(_steamOptions.Endpoints.GetOwnedGames, _steamOptions.ApiKey, _steamOptions.UserId);
        return await _httpClientFactory.GetDataUsingClientAsync<SteamOwnedGamesResponse>(ApiConstants.SteamClientName, endpoint, cancellationToken);
    }

    private async Task<SteamSchemaForGameResponse> GetGameSchemaAsync(int appId, CancellationToken cancellationToken)
    {
        var endpoint = string.Format(_steamOptions.Endpoints.GetSchemaForGame, _steamOptions.ApiKey, _steamOptions.UserId, appId);
        var gameSchema = await _httpClientFactory.GetDataUsingClientAsync<SteamSchemaForGameResponse>(ApiConstants.SteamClientName, endpoint, cancellationToken);

        // Some games return empty content as no achievements/schema - need to manually set the inner values toa void errors
        if (gameSchema.Root?.Stats?.Achievements == null)
        {
            gameSchema = new SteamSchemaForGameResponse
            (
                new SteamSchemaForGameRoot(string.Empty, new SteamSchemaForGameStats([]))
            );
        }

        return gameSchema;
    }

    private async Task<SteamPlayerAchievementsResponse> GetGamePlayerAchievementsAsync(bool gameHasAchievements, int appId, CancellationToken cancellationToken)
    {
        if (!gameHasAchievements)
        {
            return new SteamPlayerAchievementsResponse
            (
                new SteamPlayerAchievementsRoot(Error: "No stats", false, [])
            );
        }

        var endpoint = string.Format(_steamOptions.Endpoints.GetPlayerAchievements, _steamOptions.ApiKey, _steamOptions.UserId, appId);
        return await _httpClientFactory.GetDataUsingClientAsync<SteamPlayerAchievementsResponse>(ApiConstants.SteamClientName, endpoint, cancellationToken);
    }

    private async Task<SteamGlobalGameAchievementsResponse> GetGameGlobalAchievementsAsync(bool gameHasAchievements, int appId, CancellationToken cancellationToken)
    {
        if (!gameHasAchievements)
        {
            return new SteamGlobalGameAchievementsResponse
            (
                new SteamGlobalGameAchievementsRoot([])
            );
        }

        var gameId = appId;
        var endpoint = string.Format(_steamOptions.Endpoints.GetGlobalGameAchievements, _steamOptions.ApiKey, _steamOptions.UserId, appId, gameId);
        return await _httpClientFactory.GetDataUsingClientAsync<SteamGlobalGameAchievementsResponse>(ApiConstants.SteamClientName, endpoint, cancellationToken);
    }
}

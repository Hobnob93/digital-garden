using DigitalGarden.Data.Dtos;
using DigitalGarden.Data.Sync.Models;
using DigitalGarden.Shared.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DigitalGarden.Data.Sync;

public class SyncRecentLifeLogsContent : ISyncContent
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SyncRecentLifeLogsContent> _logger;

    public SyncRecentLifeLogsContent(ApplicationDbContext dbContext, ILogger<SyncRecentLifeLogsContent> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SynchronizeAsync(JsonSerializerOptions serializerOptions, string contentRootPath, CancellationToken cancellationToken)
    {
        var beaconsFilePath = $"{contentRootPath}/recent-life-logs.json";
        if (!File.Exists(beaconsFilePath))
        {
            _logger.LogError("JSON file in {ContentClass} not found.", nameof(SyncRecentLifeLogsData));
            return;
        }

        var json = await File.ReadAllTextAsync(beaconsFilePath, cancellationToken);
        var recentLogsData = JsonSerializer.Deserialize<SyncRecentLifeLogsData[]>(json, serializerOptions);

        if (recentLogsData is null)
        {
            _logger.LogError("Parsing of JSON in {ContentClass} failed.", nameof(SyncRecentLifeLogsData));
            return;
        }

        await SyncRecentLogsAsync(recentLogsData, cancellationToken);
    }

    private async Task SyncRecentLogsAsync(SyncRecentLifeLogsData[] recentLogsToSync, CancellationToken cancellationToken)
    {
        var itemSlugsFromSync = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var storedQuotes = (await _dbContext.RecentLifeLogItems
            .ToArrayAsync(cancellationToken))
            .ToDictionary(c => c.Slug, StringComparer.OrdinalIgnoreCase);

        foreach (var recentLogToSync in recentLogsToSync)
        {
            if (itemSlugsFromSync.Contains(recentLogToSync.UrlSlug))
                throw new InvalidOperationException($"Duplicate quote slug '{recentLogToSync.UrlSlug}'");

            itemSlugsFromSync.Add(recentLogToSync.UrlSlug);

            if (!storedQuotes.TryGetValue(recentLogToSync.UrlSlug, out var lifeLogItem))
            {
                lifeLogItem = new LifeLogItemDto
                {
                    Id = Guid.NewGuid(),
                    Title = recentLogToSync.Name,
                    Description = recentLogToSync.Description,
                    Type = Enum.Parse<LifeLogItemType>(recentLogToSync.Type),
                    IsCurrent = recentLogToSync.IsCurrent,
                    Slug = recentLogToSync.UrlSlug,
                    AddedAtUtc = DateTime.UtcNow
                };

                await _dbContext.RecentLifeLogItems.AddAsync(lifeLogItem, cancellationToken);
            }
            else
            {
                lifeLogItem.Title = recentLogToSync.Name;
                lifeLogItem.Description = recentLogToSync.Description;
                lifeLogItem.Type = Enum.Parse<LifeLogItemType>(recentLogToSync.Type);
                lifeLogItem.IsCurrent = recentLogToSync.IsCurrent;
            }
        }

        // No deletion on slug match fail - data will stick around for historical purposes

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

using DigitalGarden.Data.Dtos;
using DigitalGarden.Data.Sync.Models;
using DigitalGarden.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;

namespace DigitalGarden.Data.Sync;

public class SyncBeaconsContent : ISyncContent
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SyncBeaconsContent> _logger;

    public SyncBeaconsContent(ApplicationDbContext dbContext, ILogger<SyncBeaconsContent> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SynchronizeAsync(JsonSerializerOptions serializerOptions, string contentRootPath, CancellationToken cancellationToken)
    {
        var beaconsFilePath = $"{contentRootPath}/beacons.json";
        if (!File.Exists(beaconsFilePath))
        {
            _logger.LogError("JSON file '{File}' in {ContentClass} not found.", beaconsFilePath, nameof(SyncBeaconsContent));
            _logger.LogWarning("Files list: {Files}", string.Join(",", Directory.GetFiles(contentRootPath)));
            return;
        }

        var json = await File.ReadAllTextAsync(beaconsFilePath, cancellationToken);
        var rootData = JsonSerializer.Deserialize<SyncBeaconsRootData>(json, serializerOptions);

        if (rootData is null)
        {
            _logger.LogError("Parsing of JSON in {ContentClass} failed.", nameof(SyncBeaconsContent));
            return;
        }

        await SyncBeaconCategoriesAsync(rootData.Categories, cancellationToken);
        await SyncBeaconsAsync(rootData.BeaconsInCategories, cancellationToken);
    }

    private async Task SyncBeaconCategoriesAsync(SyncBeaconCategoryData[] categoriesToSync, CancellationToken cancellationToken)
    {
        var categorySlugsFromSync = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var storedCategories = (await _dbContext.BeaconCategories
            .ToArrayAsync(cancellationToken))
            .ToDictionary(c => c.Slug, StringComparer.OrdinalIgnoreCase);

        foreach (var categoryToSync in categoriesToSync)
        {
            categorySlugsFromSync.Add(categoryToSync.UrlSlug);

            if (!storedCategories.TryGetValue(categoryToSync.UrlSlug, out var category))
            {
                category = new BeaconCategoryDto
                {
                    Name = categoryToSync.Name,
                    Description = categoryToSync.Description,
                    Slug = categoryToSync.UrlSlug,
                    SortOrder = categoryToSync.SortOrder
                };

                await _dbContext.BeaconCategories.AddAsync(category, cancellationToken);
            }
            else
            {
                category.Name = categoryToSync.Name;
                category.Description = categoryToSync.Description;
                category.SortOrder = categoryToSync.SortOrder;
            }
        }

        var deletionTargets = await _dbContext.BeaconCategories
            .Where(dto => !categorySlugsFromSync.Contains(dto.Slug))
            .ToArrayAsync(cancellationToken);

        if (deletionTargets.Length > 0)
            _dbContext.BeaconCategories.RemoveRange(deletionTargets);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SyncBeaconsAsync(Dictionary<string, SyncBeaconData[]> beaconsInCategoriesToSync, CancellationToken cancellationToken)
    {
        var beaconSlugsFromSync = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var storedCategories = (await _dbContext.BeaconCategories
            .ToArrayAsync(cancellationToken))
            .ToDictionary(c => c.Slug, StringComparer.OrdinalIgnoreCase);

        var storedBeacons = (await _dbContext.Beacons
            .ToArrayAsync(cancellationToken))
            .ToDictionary(c => c.Slug, StringComparer.OrdinalIgnoreCase);

        foreach (var beaconsInCategoryToSync in beaconsInCategoriesToSync)
        {
            var categorySlug = beaconsInCategoryToSync.Key.ToSlugString();

            var category = storedCategories.GetValueOrDefault(categorySlug);
            if (category is null)
            {
                _logger.LogError("Could not find category slug {CategorySlug} in {ContentClass}.", categorySlug, nameof(SyncBeaconsContent));
                continue;
            }

            foreach (var beaconToSync in beaconsInCategoryToSync.Value)
            {
                beaconSlugsFromSync.Add(beaconToSync.UrlSlug);

                if (!storedBeacons.TryGetValue(beaconToSync.UrlSlug, out var beacon))
                {
                    beacon = new BeaconDto
                    {
                        CategoryId = category.Id,
                        Title = beaconToSync.Name,
                        Description = beaconToSync.Description,
                        Url = beaconToSync.Url,
                        Slug = beaconToSync.UrlSlug,
                        AddedAtUtc = DateTime.UtcNow
                    };

                    await _dbContext.Beacons.AddAsync(beacon, cancellationToken);
                }
                else
                {
                    beacon.CategoryId = category.Id;
                    beacon.Title = beaconToSync.Name;
                    beacon.Description = beaconToSync.Description;
                    beacon.Url = beaconToSync.Url;
                }
            }
        }

        var deletionTargets = await _dbContext.Beacons
            .Where(dto => !beaconSlugsFromSync.Contains(dto.Slug))
            .ToArrayAsync(cancellationToken);

        if (deletionTargets.Length > 0)
            _dbContext.Beacons.RemoveRange(deletionTargets);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

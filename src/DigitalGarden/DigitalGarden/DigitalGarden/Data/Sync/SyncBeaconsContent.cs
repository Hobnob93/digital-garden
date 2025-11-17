using DigitalGarden.Data.Dtos;
using DigitalGarden.Data.Sync.Models;
using DigitalGarden.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
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
            _logger.LogError("JSON file in {ContentClass} not found.", nameof(SyncBeaconsContent));
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
        var categorySlugs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var categoryToSync in categoriesToSync)
        {
            categorySlugs.Add(categoryToSync.UrlSlug);

            var category = await _dbContext.BeaconCategories
                .FirstOrDefaultAsync(bc => EF.Functions.ILike(bc.Slug, categoryToSync.UrlSlug), cancellationToken);

            if (category is null)
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
            .Where(dto => !categorySlugs.Contains(dto.Slug))
            .ToArrayAsync(cancellationToken);

        if (deletionTargets.Length > 0)
            _dbContext.BeaconCategories.RemoveRange(deletionTargets);

        await _dbContext.SaveChangesAsync();
    }

    private async Task SyncBeaconsAsync(Dictionary<string, SyncBeaconData[]> beaconsInCategoriesToSync, CancellationToken cancellationToken)
    {
        var beaconSlugs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var beaconsInCategoryToSync in beaconsInCategoriesToSync)
        {
            var categorySlug = beaconsInCategoryToSync.Key.ToSlugString();

            var category = await _dbContext.BeaconCategories
                .FirstOrDefaultAsync(bc => EF.Functions.ILike(bc.Slug, categorySlug), cancellationToken);

            if (category is null)
            {
                _logger.LogError("Could not find category slug {CategorySlug} in {ContentClass}.", categorySlug, nameof(SyncBeaconsContent));
                continue;
            }

            foreach (var beaconToSync in beaconsInCategoryToSync.Value)
            {
                beaconSlugs.Add(beaconToSync.UrlSlug);

                var beacon = await _dbContext.Beacons
                    .FirstOrDefaultAsync(b => EF.Functions.ILike(b.Slug, beaconToSync.UrlSlug), cancellationToken);

                if (beacon is null)
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
            .Where(dto => !beaconSlugs.Contains(dto.Slug))
            .ToArrayAsync(cancellationToken);

        if (deletionTargets.Length > 0)
            _dbContext.Beacons.RemoveRange(deletionTargets);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

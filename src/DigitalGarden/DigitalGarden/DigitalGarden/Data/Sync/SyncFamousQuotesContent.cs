using DigitalGarden.Data.Sync.Models;
using System.Text.Json;

namespace DigitalGarden.Data.Sync;

public class SyncFamousQuotesContent : ISyncContent
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SyncBeaconsContent> _logger;

    public SyncFamousQuotesContent(ApplicationDbContext dbContext, ILogger<SyncBeaconsContent> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SynchronizeAsync(JsonSerializerOptions serializerOptions, string contentRootPath, CancellationToken cancellationToken)
    {
        var beaconsFilePath = $"{contentRootPath}/famous-quotes.json";
        if (!File.Exists(beaconsFilePath))
        {
            _logger.LogError("JSON file in {ContentClass} not found.", nameof(SyncFamousQuotesContent));
            return;
        }

        var json = await File.ReadAllTextAsync(beaconsFilePath, cancellationToken);
        var quotesData = JsonSerializer.Deserialize<SyncFamousQuoteData[]>(json, serializerOptions);

        if (quotesData is null)
        {
            _logger.LogError("Parsing of JSON in {ContentClass} failed.", nameof(SyncFamousQuotesContent));
            return;
        }

        await SyncFamousQuotesAsync(quotesData, cancellationToken);
    }

    private async Task SyncFamousQuotesAsync(SyncFamousQuoteData[] quotesToSync, CancellationToken cancellationToken)
    {
        var itemSlugs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var quoteToSync in quotesToSync)
        {
            if (itemSlugs.Contains(quoteToSync.Slug))
                throw new InvalidOperationException($"Duplicate quote slug '{quoteToSync.Slug}'");

            itemSlugs.Add(quoteToSync.Slug);
        }
    }
}
